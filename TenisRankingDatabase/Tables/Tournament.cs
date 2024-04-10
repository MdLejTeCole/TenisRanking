namespace TenisRankingDatabase.Tables;

public class Tournament
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public bool AllMatches { get; set; }
    public int NumberOfMatchesPerPlayer { get; set; }
    public int NumberOfSets { get; set; }
    public bool TieBreak { get; set; }
    public bool ExtraPointsForTournamentWon { get; set; } = false;
    public int ExtraPoints1Place { get; set; }
    public int ExtraPoints2Place { get; set; }
    public int ExtraPoints3Place { get; set; }
    public virtual List<TournamentPlayer> TournamentPlayers { get; set; } = null!;
    public virtual List<Match> Matches { get; set; } = null!;
}

