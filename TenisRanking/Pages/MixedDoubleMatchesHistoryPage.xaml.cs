using GameTools.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TenisRankingDatabase.Enums;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameTools.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MixedDoubleMatchesHistoryPage : ExtendedPage
{
    public MixedDoubleMatchesHistoryPage()
    {
        this.InitializeComponent();
    }

    protected override void GetValuesFromDatabase()
    {
        MyDataGrid.ItemsSource = null;
        var matchesHistory = DbContext.Matches
            .Include(x => x.Tournament)
            .Include(x => x.PlayerMatches)
                .ThenInclude(x => x.Player)
            .Where(x => x.Tournament.TenisMatchType == TenisMatchType.MixedDouble)
            .OrderByDescending(x => x.Id)
            .Select(x => DoubleMatchHistory.Create(x));
        MyDataGrid.ItemsSource = matchesHistory;
    }
}
