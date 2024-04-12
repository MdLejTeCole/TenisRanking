using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TenisRanking;
using TenisRankingDatabase;
using TenisRankingDatabase.Tables;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameTools.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayersPage : ExtendedPage
    {

        public PlayersPage()
        {
            this.InitializeComponent();
        }

        protected override void GetValuesFromDatabase()
        {
            MyDataGrid.ItemsSource = null;
            MyDataGrid.ItemsSource = DbContext.Players.Where(x => x.Id > 1).ToList();
        }

        private void MyDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            var header = e.Column.Header;
            List<Player> orderPlayers = null;
            switch (header)
            {
                case "Id":
                    orderPlayers = (MyDataGrid.ItemsSource as List<Player>).OrderBy(x => x.Id).ToList();
                    break;
                case "Imiê":
                    orderPlayers = (MyDataGrid.ItemsSource as List<Player>).OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();
                    break;
                case "Nazwisko":
                    orderPlayers = (MyDataGrid.ItemsSource as List<Player>).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
                    break;
                case "Pseudonim":
                    orderPlayers = (MyDataGrid.ItemsSource as List<Player>).OrderBy(x => x.Nick).ToList();
                    break;
            }
            if (orderPlayers != null)
            {
                MyDataGrid.ItemsSource = null;
                MyDataGrid.ItemsSource = orderPlayers;
            }
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            MyDataGrid.CommitEdit();
            var button = sender as Button;
            var player = button.DataContext as Player;
            try
            {
                DbContext.Players.Update(player);
                DbContext.SaveChanges();
                ShowInfoBar(SuccessInfoBar);
            }
            catch (Exception)
            {
                ShowInfoBar(FailedInfoBar);
            }
        }
    }
}
