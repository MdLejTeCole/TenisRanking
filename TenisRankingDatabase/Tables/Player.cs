namespace TenisRankingDatabase.Tables;

public class Player
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Nick { get; set; } = string.Empty;
    public int Elo { get; set; }
    public int WinMatches { get; set; }
    public int LostMeches { get; set; }
    public int Draw { get; set; }
    public int WinTournaments { get; set; }
    public virtual List<TournamentPlayer> TournamentPlayers { get; set; } = null!;
    public virtual List<PlayerMatch> PlayerMatches { get; set; } = null!;
}

