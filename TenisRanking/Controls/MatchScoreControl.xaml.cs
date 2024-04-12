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
using TenisRankingDatabase;
using System.ComponentModel;
using TenisRankingDatabase.Tables;
using Microsoft.EntityFrameworkCore;
using TenisRankingDatabase.Enums;
using GameTools.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameTools.Controls
{
    public sealed partial class MatchScoreControl : UserControl, INotifyPropertyChanged
    {
        private readonly TenisRankingDbContext _dbContext;
        private readonly CalculateMatchScore _calculateMatchScore;

        private Match _match;

        public Match Match
        {
            get { return _match; }
            set
            {
                if (_match != value)
                {
                    _match = value;
                    OnPropertyChanged(nameof(Match));
                }
            }
        }

        public bool Set2Enabled { get; set; }
        public bool Set3Enabled { get; set; }

        public long MatchId { get; set; }

        public MatchResult MatchResult { get; set; }
        public MatchWinnerResult MatchWinnerResult { get; set; }
        protected MatchScoreControl()
        {
            this.InitializeComponent();
        }

        public MatchScoreControl(TenisRankingDbContext dbContext, long matchId) : this()
        {
            _dbContext = dbContext;
            _calculateMatchScore = new CalculateMatchScore(_dbContext);
            Match = _dbContext.Matches
                .Include(x => x.PlayerMatches)
                    .ThenInclude(x => x.Player)
                .First(x => x.Id == matchId);
            if (Match.PlayerMatches.Count == 1)
            {
                Match.PlayerMatches.Add(new PlayerMatch());
            }
            if (Match.Confirmed)
            {
                MatchResult = Match.MatchResult;
                MatchWinnerResult = Match.MatchWinnerResult;
            }
            var tournament = _dbContext.Matches.Include(x => x.Tournament).Select(x => x.Tournament).First();
            Set2Enabled = tournament.NumberOfSets >= 2;
            Set3Enabled = tournament.NumberOfSets >= 3;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void UpdateMatchResult(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem)
            {
                if (menuFlyoutItem.Tag is MatchResult matchResult)
                {
                    DropDownMatchResult.Content = menuFlyoutItem.Text;
                    MatchResult = matchResult;
                }
            }
        }

        private void UpdateWinnerResult(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem)
            {
                if (menuFlyoutItem.Tag is MatchWinnerResult matchWinnerResult)
                {
                    WinnerResultMatchResult.Content = menuFlyoutItem.Text;
                    MatchWinnerResult = matchWinnerResult;
                }
            }
        }

        private void ConfirmMatchResult(object sender, RoutedEventArgs e)
        {
            _calculateMatchScore.CalculateAndSaveMatchScore(Match, MatchResult, MatchWinnerResult);
        }

        private void DropDownMatchResult_Loaded(object sender, RoutedEventArgs e)
        {
            var menuFlyout = DropDownMatchResult.Flyout as MenuFlyout;
            if (menuFlyout != null)
            {
                foreach (MenuFlyoutItem item in menuFlyout.Items)
                {
                    if (item is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is MatchResult matchResult && matchResult == MatchResult)
                    {
                        DropDownMatchResult.Content = menuFlyoutItem.Text;
                        break;
                    }
                }
            }
        }

        private void WinnerResultMatchResult_Loaded(object sender, RoutedEventArgs e)
        {
            var menuFlyout = WinnerResultMatchResult.Flyout as MenuFlyout;
            if (menuFlyout != null)
            {
                foreach (MenuFlyoutItem item in menuFlyout.Items)
                {
                    if (item is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is MatchWinnerResult matchWinnerResult && matchWinnerResult == MatchWinnerResult)
                    {
                        WinnerResultMatchResult.Content = menuFlyoutItem.Text;
                        break;
                    }
                }
            }
        }
    }
}
