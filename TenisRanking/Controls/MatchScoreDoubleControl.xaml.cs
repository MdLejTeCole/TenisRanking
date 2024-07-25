using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TenisRankingDatabase;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using TenisRankingDatabase.Enums;
using GameTools.Pages;
using Microsoft.UI;
using GameTools.Models;
using System.Threading.Tasks;
using GameTools.Services.Double;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameTools.Controls;

public sealed partial class MatchScoreDoubleControl : UserControl, INotifyPropertyChanged
{
    private readonly TenisRankingDbContext _dbContext;
    private readonly DoubleCalculateMatchScore _calculateMatchScore;
    private readonly MixedDoubleMatchesPage _matchesPage;
    public event PropertyChangedEventHandler PropertyChanged;

    private MixDoubleMatchDto _match;
    public MixDoubleMatchDto Match
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

    private readonly SolidColorBrush _defaultColor = new SolidColorBrush(Colors.DarkGray);
    private readonly SolidColorBrush _greenDefaultColor = new SolidColorBrush(Colors.SlateGray);
    private readonly SolidColorBrush _greenColor = new SolidColorBrush(Colors.YellowGreen);
    private readonly SolidColorBrush _yellowColor = new SolidColorBrush(Colors.SandyBrown);

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

    public int Sets { get; set; }
    public bool Set2Enabled { get; set; }
    public bool Set3Enabled { get; set; }

    public MatchResult MatchResult { get; set; }
    public MatchWinnerResult MatchWinnerResult { get; set; }

    public MatchScoreDoubleControl()
    {
        this.InitializeComponent();
    }

    public MatchScoreDoubleControl(TenisRankingDbContext dbContext, MixedDoubleMatchesPage matchesPage, long matchId) : this()
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _matchesPage = matchesPage ?? throw new ArgumentNullException(nameof(matchesPage));
        _calculateMatchScore = new DoubleCalculateMatchScore(_dbContext);
        Loaded += Page_Loaded;

        AssignMatch(matchId);
        AssignTurnamentSettings(matchId);
    }

    private void AssignMatch(long matchId)
    {
        Match = MixDoubleMatchDto.Create(_dbContext.Matches
            .Include(x => x.PlayerMatches)
                .ThenInclude(x => x.Player)
            .First(x => x.Id == matchId));
        if (Match.Confirmed)
        {
            MatchResult = Match.MatchResult;
            MatchWinnerResult = Match.MatchWinnerResult;
        }
    }

    private void AssignTurnamentSettings(long matchId)
    {
        var tournament = _dbContext.Matches.Include(x => x.Tournament).First(x => x.Id == matchId).Tournament;
        Sets = tournament.NumberOfSets;
        Set2Enabled = tournament.NumberOfSets >= 2;
        Set3Enabled = tournament.NumberOfSets >= 3;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (Match.Confirmed)
        {
            Color = _greenColor;
        }
        else if (Match.Confirmed && Match.MatchWinnerResult == MatchWinnerResult.None)
        {
            Color = _greenDefaultColor;
        }
        else
        {
            Color = _defaultColor;
        }
    }

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
        if (result)
        {
            Match.Confirmed = true;
            if (MatchWinnerResult != MatchWinnerResult.None)
            {
                Color = _greenColor;
            }
            else
            {
                Color = _greenDefaultColor;
            }
        }
        _matchesPage.ShowNotification(result);
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

    private async  void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (Double.IsNaN(sender.Value))
        {
            sender.Value = 0;
            sender.Text = "0";
        }
        Color = _yellowColor;

        await Task.Delay(200);

        UpdateWinner();
    }

    private void UpdateWinner()
    { 
        if (_match.Confirmed)
        {
            return;
        }
        var sets = new List<bool?>
        {
            FirstPlayerWonSet(_match.Team1.Set1, _match.Team2.Set1),
            FirstPlayerWonSet(_match.Team1.Set2, _match.Team2.Set2),
            FirstPlayerWonSet(_match.Team1.Set3, _match.Team2.Set3),
            FirstPlayerWonSet(_match.Team1.Set4, _match.Team2.Set4),
            FirstPlayerWonSet(_match.Team1.Set5, _match.Team2.Set5)
        };
        if (sets.All(x => !x.HasValue))
        {
            return;
        }
        else if (sets.Where(x => x.HasValue && x.Value == true).Count() > sets.Where(x => x.HasValue && x.Value == false).Count() && sets.Where(x => x.HasValue && x.Value == true).Count() == Sets)
        {
            MatchResult = MatchResult.Finished;
            MatchWinnerResult = MatchWinnerResult.FirstPlayerWin;
            DropDownMatchResult_Loaded(DropDownMatchResult, null);
            WinnerResultMatchResult_Loaded(WinnerResultMatchResult, null);
        }
        else if (sets.Where(x => x.HasValue && x.Value == true).Count() < sets.Where(x => x.HasValue && x.Value == false).Count() && sets.Where(x => x.HasValue && x.Value == false).Count() == Sets)
        {
            MatchResult = MatchResult.Finished;
            MatchWinnerResult = MatchWinnerResult.SecondPlayerWin;
            DropDownMatchResult_Loaded(DropDownMatchResult, null);
            WinnerResultMatchResult_Loaded(WinnerResultMatchResult, null);
        }
    }

    private bool? FirstPlayerWonSet(int? setFirstPlayer, int? setSecondPlayer)
    {
        if (setFirstPlayer != null && setSecondPlayer != null)
        {
            if (setFirstPlayer > setSecondPlayer)
            {
                return true;
            }
            else if (setFirstPlayer < setSecondPlayer)
            {
                return false;
            }
        }
        return null;
    }

    private async void DeleteMatch(object sender, RoutedEventArgs e)
    {
        var result = await ShowConfirmationDialog($"Czy na pewno chcesz usunac mecz?");

        if (result == ContentDialogResult.Primary)
        {
            _dbContext.Matches.Remove(_dbContext.Matches.First(x => x.Id == Match.Id));
            _dbContext.SaveChanges();
            _matchesPage.SetAfterChangePage();
        }
    }

    private async Task<ContentDialogResult> ShowConfirmationDialog(string content)
    {
        ContentDialog confirmationDialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = "Potwierdzenie",
            Content = content,
            PrimaryButtonText = "Tak",
            CloseButtonText = "Anuluj"
        };

        return await confirmationDialog.ShowAsync();
    }

    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
