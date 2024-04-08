using Microsoft.EntityFrameworkCore;
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
    public sealed partial class TournamentPage : Page
    {
        private TenisRankingDbContext _dbContext;
        private List<Player> _allPlayers;
        public ObservableCollection<Player> Players { get; set; } = new ObservableCollection<Player>();

        public TournamentPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is TenisRankingDbContext dbContext)
            {
                _dbContext = dbContext;
                _allPlayers = _dbContext.Players.ToList();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Player player)
            {
                Players.Remove(player);
                _allPlayers.Add(player);
            }
        }

        private void PlayerAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = _allPlayers.Where(player =>
                    player.FirstName.ToLower().Contains(sender.Text.ToLower()) ||
                    player.LastName.ToLower().Contains(sender.Text.ToLower()) ||
                    player.Nick.ToLower().Contains(sender.Text.ToLower()))
                    .Select(x => $"{x.FirstName} {x.LastName} {x.Elo} ({x.Nick})").ToList();
            }
        }

        private void PlayerAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string selectedPlayerString)
            {
                var playerInfo = selectedPlayerString.Split(' ');
                var nick = playerInfo[3].TrimStart('(').TrimEnd(')');
                var player = _allPlayers.First(x => x.Nick == nick);
                Players.Add(player);
                _allPlayers.Remove(player);
                sender.Text = "";
            }
        }
    }
}
