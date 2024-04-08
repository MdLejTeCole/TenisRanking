using CommunityToolkit.WinUI.UI.Controls;
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
    public sealed partial class PlayersPage : Page
    {
        private TenisRankingDbContext _dbContext;

        public PlayersPage()
        {
            this.InitializeComponent();
        }

        private void UpdatePlayersTable()
        {
            MyDataGrid.ItemsSource = null;
            MyDataGrid.ItemsSource = _dbContext.Players.ToList();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is TenisRankingDbContext dbContext)
            {
                _dbContext = dbContext;
                UpdatePlayersTable();
            }
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
                case "Elo":
                    orderPlayers = (MyDataGrid.ItemsSource as List<Player>).OrderByDescending(x => x.Elo).ToList();
                    break;
                case "Wygrane mecze":
                    orderPlayers = (MyDataGrid.ItemsSource as List<Player>).OrderByDescending(x => x.WinMatches).ToList();
                    break;
                case "Przegrane mecze":
                    orderPlayers = (MyDataGrid.ItemsSource as List<Player>).OrderByDescending(x => x.LostMatches).ToList();
                    break;
                case "Remisy":
                    orderPlayers = (MyDataGrid.ItemsSource as List<Player>).OrderByDescending(x => x.Draw).ToList();
                    break;
                case "Wygrane turnieje":
                    orderPlayers = (MyDataGrid.ItemsSource as List<Player>).OrderByDescending(x => x.WinTournaments).ToList();
                    break;
            }
            if (orderPlayers != null)
            {
                MyDataGrid.ItemsSource = null;
                MyDataGrid.ItemsSource = orderPlayers;
            }
            //.OrderBy(x => x.FirstName).ThenBy(c => c.LastName)
        }
    }
}
