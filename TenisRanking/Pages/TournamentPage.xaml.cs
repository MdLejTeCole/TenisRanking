using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
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
using Windows.Networking;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameTools.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TournamentPage : ExtendedPage
    {
        private List<Player> _allPlayers;
        public ObservableCollection<Player> Players { get; set; } = new ObservableCollection<Player>();

        private Tournament _tournament;

        public Tournament Tournament
        {
            get { return _tournament; }
            set
            {
                if (_tournament != value)
                {
                    _tournament = value;
                    OnPropertyChanged(nameof(Tournament));
                }
            }
        }

        public TournamentPage()
        {
            this.InitializeComponent();
        }

        protected override void GetValuesFromDatabase()
        {
            _allPlayers = DbContext.Players.ToList();
            var settings = DbContext.Settings.First();
            Tournament = new Tournament()
            {
                AllMatches = settings.AllMatches,
                NumberOfMatchesPerPlayer = settings.NumberOfMatchesPerPlayer,
                NumberOfSets = settings.NumberOfSets,
                TieBreak = settings.TieBreak,
                ExtraPointsForTournamentWon = settings.ExtraPointsForTournamentWon,
                ExtraPoints1Place = settings.ExtraPoints1Place,
                ExtraPoints2Place = settings.ExtraPoints2Place,
                ExtraPoints3Place = settings.ExtraPoints3Place
            };
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

        private void CreateTournament(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Name.Text))
            {
                ShowInfoBar(MissingValuesInfoBar);
                return;
            }
            if (Players.Count < 2)
            {
                ShowInfoBar(MissingPlayersInfoBar);
                return;
            }
            try
            {
                if (DbContext.Tournaments.Any(x => x.Name == Name.Text))
                {
                    ShowInfoBar(NotUniqueInfoBar);
                    return;
                }
                var transaction = DbContext.Database.BeginTransaction();
                Tournament.Name = Name.Text;
                Tournament.Date = DateOnly.FromDateTime(DateTime.UtcNow);
                DbContext.Tournaments.Add(Tournament);
                DbContext.SaveChanges();
                var tournamentPlayers = new List<TournamentPlayer>();
                foreach (var player in Players)
                {
                    tournamentPlayers.Add(new TournamentPlayer() { TournamentId = Tournament.Id, PlayerId = player.Id });
                }
                DbContext.TournamentPlayers.AddRange(tournamentPlayers);
                DbContext.SaveChanges();
                transaction.Commit();
                ShowInfoBar(SuccessInfoBar);
            }
            catch (Exception)
            {
                DbContext.Database.RollbackTransaction();
                ShowInfoBar(FailedInfoBar);
            }
        }
    }
}
