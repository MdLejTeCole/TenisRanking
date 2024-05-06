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
using TenisRankingDatabase.Enums;
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

        private bool _isUpdate = false;

        private int _avarageElo;

        public int AvarageElo
        {
            get { return _avarageElo; }
            set { _avarageElo = value;
                OnPropertyChanged(nameof(AvarageElo));
            }
        }

        private bool changeMatchTypeEnabled;

        public bool ChangeMatchTypeEnabled
        {
            get { return changeMatchTypeEnabled; }
            set { changeMatchTypeEnabled = value;
                OnPropertyChanged(nameof(ChangeMatchTypeEnabled));
            }
        }

        private bool isSingle;

        public bool IsSingle
        {
            get { return isSingle; }
            set { isSingle = value; 
                OnPropertyChanged(nameof(IsSingle));
            }
        }



        public TournamentPage()
        {
            IsSingle = true;
            this.InitializeComponent();
        }


        protected override void GetValuesFromDatabase()
        {        
            CreateNewTournament();
        }

        private void CreateNewTournament()
        {
            _allPlayers = DbContext.Players.Where(x => x.Id > 1).ToList();
            Players.Clear();
            var settings = DbContext.Settings.First();
            Tournament = new Tournament()
            {
                NumberOfSets = settings.NumberOfSets,
                ExtraPointsForTournamentWon = settings.ExtraPointsForTournamentWon,
                ExtraPoints1Place = settings.ExtraPoints1Place,
                ExtraPoints2Place = settings.ExtraPoints2Place,
                ExtraPoints3Place = settings.ExtraPoints3Place
            };
            ChangeMatchTypeEnabled = true;
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
                AvarageElo = (int)Players.Select(x => x.Elo).Average();
            }
        }

        private void CreateTournament(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Name.Text))
            {
                ShowInfoBar(MissingValuesInfoBar);
                return;
            }
            if ((Players.Count < 2 && Tournament.TenisMatchType == TenisMatchType.Single) || (Players.Count < 4 && Tournament.TenisMatchType == TenisMatchType.MixedDouble))
            {
                ShowInfoBar(MissingPlayersInfoBar);
                return;
            }
            try
            {
                if (DbContext.Tournaments.Any(x => x.Name == Name.Text) && !_isUpdate)
                {
                    ShowInfoBar(NotUniqueInfoBar);
                    return;
                }
                var transaction = DbContext.Database.BeginTransaction();
                Tournament.Name = Name.Text;
                Tournament.AvarageElo = AvarageElo;
                if (!_isUpdate)
                {
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
                }
                else
                {
                    Tournament.TournamentPlayers.RemoveAll(x => !Players.Any(y => y == x.Player));
                    foreach (var player in Players)
                    {
                        if (Tournament.TournamentPlayers.Any(x => x.Player == player))
                        {
                            continue;
                        }
                        if (!Tournament.TournamentPlayers.Any(x => x.Player == player))
                        {
                            Tournament.TournamentPlayers.Add(new TournamentPlayer() { TournamentId = Tournament.Id, PlayerId = player.Id });
                        }
                    }
                    DbContext.Tournaments.Update(Tournament);
                    DbContext.SaveChanges();
                }

                transaction.Commit();
                ShowInfoBar(SuccessInfoBar);
                CreateNewTournament();
                _isUpdate = false;
            }
            catch (Exception)
            {
                DbContext.Database.RollbackTransaction();
                ShowInfoBar(FailedInfoBar);
            }
        }

        private async void Name_LostFocus(object sender, RoutedEventArgs e)
        {
            var tournament = DbContext.Tournaments
                .Include(x => x.TournamentPlayers)
                    .ThenInclude(x => x.Player)
                .FirstOrDefault(x => x.Name == (sender as TextBox).Text);

            if (tournament != null)
            {
                if (tournament.TournamentStatus == TournamentStatus.Ended)
                {
                    await ShowInformationDialog($"Intnieje juz turniej zakoñczony turniej o takiej nazwie {tournament.Name}. Zmieñ nazwe turnieju na unikaln¹");
                    return;
                }
                if (await ShowConfirmationDialog($"Intnieje juz turniej o nazwie {tournament.Name}. Czy chcesz go edytowaæ?") == ContentDialogResult.Primary)
                {
                    _isUpdate = true;
                    Tournament = tournament;
                    _allPlayers = DbContext.Players.Where(x => x.Id > 1).ToList();
                    Players.Clear();
                    foreach (var player in Tournament.TournamentPlayers.Select(x => x.Player))
                    {
                        Players.Add(player);
                        _allPlayers.Remove(player);
                    }
                    IsSingle = tournament.TenisMatchType == TenisMatchType.Single;
                    ChangeMatchTypeEnabled = !DbContext.Matches.Any(x => x.TournamentId == tournament.Id);
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Tournament == null)
            {
                return;
            }
            var content = (sender as RadioButton).Content;
            switch (content)
            {
                case "Singiel":
                    Tournament.TenisMatchType = TenisMatchType.Single;
                    break;
                case "Debel mieszany":
                    Tournament.TenisMatchType = TenisMatchType.MixedDouble;
                    break;
                default:
                    break;
            }
        }
    }
}
