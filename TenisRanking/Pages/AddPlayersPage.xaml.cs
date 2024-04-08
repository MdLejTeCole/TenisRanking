using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class AddPlayersPage : Page
    {
        private TenisRankingDbContext _dbContext;

        public AddPlayersPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is TenisRankingDbContext dbContext)
            {
                _dbContext = dbContext;
            }
        }

        private void AddPlayer(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FirstName.Text) ||  string.IsNullOrEmpty(LastName.Text) || string.IsNullOrEmpty(Nick.Text))
            {
                ShowInfoBar(MissingValuesInfoBar);
                return;
            }
            try
            {
                if (_dbContext.Players.Any(x => x.FirstName == FirstName.Text && x.LastName == LastName.Text && x.Nick == Nick.Text))
                {
                    ShowInfoBar(NotUniqueInfoBar);
                    return;
                }
                var elo = _dbContext.Settings.First().StartElo;
                _dbContext.Players.Add(new Player() { FirstName = FirstName.Text, LastName = LastName.Text, Nick = Nick.Text, Elo = elo });
                _dbContext.SaveChanges();
                ShowInfoBar(SuccessInfoBar);
            }
            catch (Exception)
            {
                ShowInfoBar(FailedInfoBar);
            }
        }

        public void ShowInfoBar(InfoBar infoBar)
        {
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _ = Task.Run(() =>
            {
                dispatcherQueue.TryEnqueue(async () =>
                {
                    infoBar.IsOpen = true;
                    await Task.Delay(1000);
                    infoBar.IsOpen = false;
                });
            });
        }
    }
}
