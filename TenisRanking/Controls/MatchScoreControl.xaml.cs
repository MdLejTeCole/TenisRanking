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
using GameTools.Pages;
using Microsoft.UI;
using GameTools.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameTools.Controls
{
    public sealed partial class MatchScoreControl : UserControl, INotifyPropertyChanged
    {
        private readonly TenisRankingDbContext _dbContext;
        private readonly CalculateMatchScore _calculateMatchScore;
        private readonly MatchesPage _matchesPage;

        private MatchDto _match;

        public MatchDto Match
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
        private SolidColorBrush _defaultColor = new SolidColorBrush(Colors.DarkGray);
        private SolidColorBrush _greenColor = new SolidColorBrush(Colors.YellowGreen);
        private SolidColorBrush _yellowColor = new SolidColorBrush(Colors.SandyBrown);
        private SolidColorBrush _color;

        public SolidColorBrush Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        public bool Set2Enabled { get; set; }
        public bool Set3Enabled { get; set; }

        public long MatchId { get; set; }

        public MatchResult MatchResult { get; set; }
        public MatchWinnerResult MatchWinnerResult { get; set; }
        public MatchScoreControl()
        {
            this.InitializeComponent();
        }

        public MatchScoreControl(TenisRankingDbContext dbContext, MatchesPage matchesPage, long matchId) : this()
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _matchesPage = matchesPage ?? throw new ArgumentNullException(nameof(matchesPage));
            _calculateMatchScore = new CalculateMatchScore(_dbContext);
            Match = MatchDto.Create(_dbContext.Matches
                .Include(x => x.PlayerMatches)
                    .ThenInclude(x => x.Player)
                .First(x => x.Id == matchId));
            if (Match.Confirmed)
            {
                MatchResult = Match.MatchResult;
                MatchWinnerResult = Match.MatchWinnerResult;
            }
            var tournament = _dbContext.Matches.Include(x => x.Tournament).Select(x => x.Tournament).First();
            Set2Enabled = tournament.NumberOfSets >= 2;
            Set3Enabled = tournament.NumberOfSets >= 3;
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Match.Confirmed)
            {
                Color = _greenColor;
            }
            else
            {
                Color = _defaultColor;
            }
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
                    Color = _yellowColor;
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
                    Color = _yellowColor;
                }
            }
        }

        private void ConfirmMatchResult(object sender, RoutedEventArgs e)
        {
            var result = _calculateMatchScore.CalculateAndSaveMatchScore(Match, MatchResult, MatchWinnerResult);
            if (result.Result)
            {
                Match.Confirmed = true;
                Color = _greenColor;
            }
            _matchesPage.ShowNotification(result.Result);
            if (result.NeedRefresh)
            {
                _matchesPage.SetAfterChangePage();
            }
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

        private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (Double.IsNaN(sender.Value))
            {
                sender.Value = 0;
                sender.Text = "0";
            }
            Color = _yellowColor;
        }
    }
}
