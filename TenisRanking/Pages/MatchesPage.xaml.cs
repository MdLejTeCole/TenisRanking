using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using TenisRankingDatabase.Tables;
using GameTools.Controls;
using Microsoft.EntityFrameworkCore;
using Windows.UI;
using GameTools.Services;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameTools.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MatchesPage : ExtendedPage
    {
        public ObservableCollection<TournamentPlayer> Players { get; set; } = new ObservableCollection<TournamentPlayer>();
        private MatchGenerationService _matchGenerationService;
        private static long? _lastTournamentId;
        private static long? _minTournamentId;
        private static long? _maxTournamentId;

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

        private bool _isNextButtonEnabled;

        public bool IsNextButtonEnabled
        {
            get { return _isNextButtonEnabled; }
            set { 
                _isNextButtonEnabled = value;
                OnPropertyChanged(nameof(IsNextButtonEnabled));
            }
        }

        private bool _isPreviousButtonEnabled;

        public bool IsPreviousButtonEnabled
        {
            get { return _isPreviousButtonEnabled; }
            set
            {
                _isPreviousButtonEnabled = value;
                OnPropertyChanged(nameof(IsPreviousButtonEnabled));
            }
        }


        protected override void GetValuesFromDatabase()
        {
            _matchGenerationService = new MatchGenerationService(DbContext);
            if (_lastTournamentId is null)
            {
                Tournament = DbContext.Tournaments.OrderBy(x => x.Id).LastOrDefault();
            }
            else
            {
                Tournament = DbContext.Tournaments.First(x => x.Id == _lastTournamentId);
            }
            _minTournamentId = DbContext.Tournaments.OrderBy(x => x.Id).FirstOrDefault()?.Id;
            _maxTournamentId = DbContext.Tournaments.OrderBy(x => x.Id).LastOrDefault()?.Id;
            SetAfterChangePage();
        }

        public MatchesPage()
        {
            this.InitializeComponent();
        }

        private void EndTournament(object sender, RoutedEventArgs e)
        {

        }

        private void PreviousTournament(object sender, RoutedEventArgs e)
        {
            Tournament = DbContext.Tournaments.Where(x => x.Id < _lastTournamentId).OrderBy(x => x.Id).LastOrDefault();
            SetAfterChangePage();
        }

        private void NextTournament(object sender, RoutedEventArgs e)
        {
            Tournament = DbContext.Tournaments.Where(x => x.Id > _lastTournamentId).OrderBy(x => x.Id).FirstOrDefault();
            SetAfterChangePage();
        }

        private void SetAfterChangePage()
        {
            _lastTournamentId = Tournament?.Id;
            SetPlayers();
            SetEnableForButton();
            GetExistingMatches();
        }

        private void SetEnableForButton()
        {
            if (_lastTournamentId == _minTournamentId)
            {
                IsPreviousButtonEnabled = false;
            }
            else
            {
                IsPreviousButtonEnabled = true;
            }
            if (_lastTournamentId == _maxTournamentId)
            {
                IsNextButtonEnabled = false;
            }
            else
            {
                IsNextButtonEnabled = true;
            }
        }

        private void GetExistingMatches()
        {
            Matches.Children.Clear();
            if (Tournament is not null)
            {
                var matchesId = DbContext.Matches.Where(x => x.TournamentId == Tournament.Id).Select(x => x.Id).OrderBy(x => x);
                foreach (var id in matchesId)
                {
                    var matchScoreControl = new MatchScoreControl(DbContext, id);
                    Matches.Children.Add(matchScoreControl);
                }
            }
        }

        private void SetPlayers()
        {
            Players.Clear();
            if (_lastTournamentId is not null)
            {
                var tournamentPlayers = DbContext.TournamentPlayers
                    .Include(x => x.Player)
                        .ThenInclude(x => x.PlayerMatches)
                            .ThenInclude(x => x.Match)
                    .Where(x => x.TournamentId == _lastTournamentId)
                    .ToList();
                foreach (var player in tournamentPlayers.OrderByDescending(x => x.CalculateTournamentScoreInt()))
                {
                    Players.Add(player);
                };
            }
        }

        private void GenerateMatches(object sender, RoutedEventArgs e)
        {
            if (Matches.Children.Count() == 0)
            {
                var matchIds = _matchGenerationService.GenerateFirstRound(Tournament.Id);
                foreach (var id in matchIds) 
                {
                    var matchScoreControl = new MatchScoreControl(DbContext, id);
                    Matches.Children.Add(matchScoreControl);
                }
            }
            else
            {
                var matchIds = _matchGenerationService.GenerateNextRound(Tournament.Id);
                foreach (var id in matchIds)
                {
                    var matchScoreControl = new MatchScoreControl(DbContext, id);
                    Matches.Children.Add(matchScoreControl);
                }
            }
        }

        private int CalculateTournamentScore(Player player, long tournmanetId)
        {
            return player.PlayerMatches.Where(x => x.Match?.TournamentId == tournmanetId && x.MatchPoint != null).Select(x => x.MatchPoint).Sum() ?? 0;
        }
    }
}
