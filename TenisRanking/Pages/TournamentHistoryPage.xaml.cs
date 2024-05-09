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
public sealed partial class TournamentHistoryPage : ExtendedPage
{
    public TournamentHistoryPage()
    {
        this.InitializeComponent();
    }

    protected override void GetValuesFromDatabase()
    {
        MyDataGrid.ItemsSource = null;
        var tournamentsHistory = DbContext.Tournaments
            .Include(x => x.TournamentPlayers)
                .ThenInclude(x => x.Player)
            .Where(x => x.TournamentStatus == TournamentStatus.Ended)
            .OrderByDescending(x => x.Id)
            .Select(x => TournamentHistory.Create(x));
        MyDataGrid.ItemsSource = tournamentsHistory;
    }
}
