namespace TenisRankingDatabase.Tables;

public class Player
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Nick { get; set; } = string.Empty;
    public int Elo { get; set; }
    public int MixedDoubleElo { get; set; }
    public int TournamentsPoints { get; set; }
    public int TournamentsPlayed { get; set; }
    public int Tournament1Place { get; set; }
    public int Tournament2Place { get; set; }
    public int Tournament3Place { get; set; }
    public int WinMatches { get; set; }
    public int LostMatches { get; set; }
    public bool Active { get; set; } = true;
    public virtual List<TournamentPlayer> TournamentPlayers { get; set; } = null!;
    public virtual List<PlayerMatch> PlayerMatches { get; set; } = null!;

    public string GetFullName => $"{FirstName} {LastName}";
}

