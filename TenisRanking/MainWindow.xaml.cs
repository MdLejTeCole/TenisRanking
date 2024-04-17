using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GameTools.Pages;
using Windows.Devices.Enumeration;
using TenisRankingDatabase;
using Windows.ApplicationModel.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TenisRanking
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly TenisRankingDbContext _context;
        public MainWindow(TenisRankingDbContext context)
        {
            _context = context;
            this.InitializeComponent();
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(RankingPage), _context);
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            switch (args.InvokedItem.ToString())
            {
                case "Ranking":
                    ContentFrame.Navigate(typeof(RankingPage), _context);
                    break;
                case "Turniej":
                    ContentFrame.Navigate(typeof(TournamentPage), _context);
                    break;
                case "Mecze turnieju":
                    ContentFrame.Navigate(typeof(MatchesPage), _context);
                    break;
                case "Historia meczy":
                    ContentFrame.Navigate(typeof(MatchesHistoryPage), _context);
                    break;
                case "Zawodnicy":
                    ContentFrame.Navigate(typeof(PlayersPage), _context);
                    break;
                case "Dodaj zawodnika":
                    ContentFrame.Navigate(typeof(AddPlayerPage), _context);
                    break;
                case "Ustawienia":
                    ContentFrame.Navigate(typeof(SettingPage), _context);
                    break;
            }
        }
    }
}
