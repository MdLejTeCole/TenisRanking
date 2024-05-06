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
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TenisRankingDatabase.Enums;

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
        private CalculateAfterEndTournament _calculateAfterEndTournament;
        private static long? _lastTournamentId;
        private static long? _minTournamentId;
        private static long? _maxTournamentId;
        private static int _round = 1;

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

        private bool _isEnable;

        public bool IsEnable
        {
            get { return _isEnable; }
            set { _isEnable = value;
                OnPropertyChanged(nameof(IsEnable));
            }
        }

        private string _tournamentStatus;

        public string TournamentStatusTranslation
        {
            get { return _tournamentStatus; }
            set { _tournamentStatus = value;
                OnPropertyChanged(nameof(TournamentStatusTranslation));
            }
        }


        protected override void GetValuesFromDatabase()
        {
            _matchGenerationService = new MatchGenerationService(DbContext);
            _calculateAfterEndTournament = new CalculateAfterEndTournament(DbContext);
            if (_lastTournamentId is null)
            {
                Tournament = DbContext.Tournaments.Where(x => x.TenisMatchType == TenisMatchType.Single).OrderBy(x => x.Id).LastOrDefault();
            }
            else
            {
                Tournament = DbContext.Tournaments.Where(x => x.TenisMatchType == TenisMatchType.Single).FirstOrDefault(x => x.Id == _lastTournamentId);
            }
            _minTournamentId = DbContext.Tournaments.Where(x => x.TenisMatchType == TenisMatchType.Single).OrderBy(x => x.Id).FirstOrDefault()?.Id;
            _maxTournamentId = DbContext.Tournaments.Where(x => x.TenisMatchType == TenisMatchType.Single).OrderBy(x => x.Id).LastOrDefault()?.Id;
            SetAfterChangePage();
        }

        public MatchesPage()
        {
            this.InitializeComponent();
        }

        private async void EndTournament(object sender, RoutedEventArgs e)
        {
            if (DbContext.Matches
                .Include(x => x.Tournament)
                .Where(x => x.TournamentId == _lastTournamentId && x.Tournament.TenisMatchType == TenisMatchType.Single)
                .All(x => x.Confirmed))
            {
                var result = await ShowConfirmationDialog("Czy na pewno chcesz zakoñczyæ turniej?\nPo zakoñczeniu turnieju, nie mo¿na aktualizowaæ wyników meczy.");

                if (result == ContentDialogResult.Primary)
                {
                    var tournamentResult = _calculateAfterEndTournament.CalculateAndSaveUpdatesForTournament(Tournament.Id, false);
                    if (tournamentResult)
                    {
                        IsEnable = false;
                        SetTranslationTournamentStatus(TournamentStatus.Ended);
                    }
                }
            }
            else
            {
                ShowInfoBar(MissingConfirmationInfoBar);
            }  
        }

        private void PreviousTournament(object sender, RoutedEventArgs e)
        {
            Tournament = DbContext.Tournaments.Where(x => x.Id < _lastTournamentId && x.TenisMatchType == TenisMatchType.Single).OrderBy(x => x.Id).LastOrDefault();
            SetAfterChangePage();
        }

        private void NextTournament(object sender, RoutedEventArgs e)
        {
            Tournament = DbContext.Tournaments.Where(x => x.Id > _lastTournamentId && x.TenisMatchType == TenisMatchType.Single).OrderBy(x => x.Id).FirstOrDefault();
            SetAfterChangePage();
        }

        public void SetAfterChangePage()
        {
            _lastTournamentId = Tournament?.Id;
            SetPlayers();
            SetEnableForButton();
            GetExistingMatches();
            SetEnable();
        }

        private void SetEnable()
        {
            if (Tournament != null)
            {
                SetTranslationTournamentStatus(Tournament.TournamentStatus);
            }
            if (Tournament == null || Tournament.TournamentStatus != TournamentStatus.Started)
            {
                IsEnable = false;

            }
            else
            {
                IsEnable = true;
            }

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
            for (int i = 1; i < 6; i++)
            {
                var wrapPanel = GetWrapPanel(i, false);
                wrapPanel.Children.Clear();
            }
            if (Tournament is not null)
            {
                var matches = DbContext.Matches.Where(x => x.TournamentId == Tournament.Id).OrderBy(x => x.Id);
                foreach (var match in matches)
                {
                    var wrapPanel = GetWrapPanel(match.Round);
                    var matchScoreControl = new MatchScoreControl(DbContext, this, match.Id);
                    wrapPanel.Children.Add(matchScoreControl);
                }
                if (matches.Any())
                {
                    _round = matches.Last().Round;
                }
                else
                {
                    _round = 1;
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
                    .Include(x => x.Tournament)
                    .Where(x => x.TournamentId == _lastTournamentId && x.Tournament.TenisMatchType == TenisMatchType.Single)
                    .ToList();
                foreach (var player in tournamentPlayers.OrderByDescending(x => x.CalculateTournamentScoreInt()).ThenByDescending(x => x.CalculateWonSets()).ThenByDescending(x => x.CalculateWonGems()))
                {
                    Players.Add(player);
                };
            }
        }

        private void GenerateMatches(object sender, RoutedEventArgs e)
        {
            if (Matches1.Children.Count() == 0)
            {
                var matchIds = _matchGenerationService.GenerateFirstRound(Tournament.Id);
                foreach (var id in matchIds) 
                {
                    var matchScoreControl = new MatchScoreControl(DbContext, this, id);
                    Matches1.Children.Add(matchScoreControl);
                }
            }
            else if (DbContext.Matches
                .Include(x => x.Tournament)
                .Where(x => x.TournamentId == _lastTournamentId && x.Tournament.TenisMatchType == TenisMatchType.Single)
                .All(x => x.Confirmed))
            {
                _round += 1;
                var matchIds = _matchGenerationService.GenerateNextRound(Tournament.Id, _round);
                if (matchIds.Any())
                {
                    var wrapPanel = GetWrapPanel(_round);
                    foreach (var id in matchIds)
                    {
                        var matchScoreControl = new MatchScoreControl(DbContext, this, id);
                        wrapPanel.Children.Add(matchScoreControl);
                    }
                }
            }
            else
            {
                ShowInfoBar(MissingConfirmationInfoBar);
            }
        }

        public void ShowNotification(bool resultSucessful)
        {
            if (resultSucessful)
            {
                ShowInfoBar(SuccessInfoBar);
                SetPlayers();
            }
            else
            {
                ShowInfoBar(FailedInfoBar);
            }
        }

        private WrapPanel GetWrapPanel(int round, bool show = true)
        {
            switch (round)
            {
                case 1:
                    return Matches1;
                case 2:
                    ShowOrHideRound(Round2, show);
                    return Matches2;
                case 3:
                    ShowOrHideRound(Round3, show);
                    return Matches3;
                case 4:
                    ShowOrHideRound(Round4, show);
                    return Matches4;
                case 5:
                    ShowOrHideRound(Round5, show);
                    return Matches5;
                default:
                    return Matches5;
            }
        }

        private void ShowOrHideRound(TextBlock textblock, bool show)
        {
            if (show)
            {
                textblock.Visibility = Visibility.Visible;
            }
            else
            {
                textblock.Visibility = Visibility.Collapsed;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TournamentPlayer player)
            {
                DbContext.TournamentPlayers.Update(player);
                DbContext.SaveChanges();
            }
        }

        private Visibility VisableRound(int round)
        {
            if (round <= _round)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        private async void CancelTournament(object sender, RoutedEventArgs e)
        {
            var result = await ShowConfirmationDialog("Czy na pewno chcesz anulowaæ turniej?\nPo anulowaniu turnieju, punkty oraz elo meczy nie zostanie podliczone.");

            if (result == ContentDialogResult.Primary)
            {
                Tournament.TournamentStatus = TournamentStatus.Cancelled;
                DbContext.Tournaments.Update(Tournament);
                DbContext.SaveChanges();
                IsEnable = false;
                SetTranslationTournamentStatus(TournamentStatus.Cancelled);
            }

        }

        private void SetTranslationTournamentStatus(TournamentStatus tournamentStatus)
        {
            switch (tournamentStatus)
            {
                case TournamentStatus.Started:
                    TournamentStatusTranslation = "Rozpoczêty";
                    break;
                case TournamentStatus.Ended:
                    TournamentStatusTranslation = "Zakoñczony";
                    break;
                case TournamentStatus.Cancelled:
                    TournamentStatusTranslation = "Anulowany";
                    break;
            }
        }
    }
}
