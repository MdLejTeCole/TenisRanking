using TenisRankingDatabase.Enums;

namespace TenisRankingDatabase.Tables;

public class PlayerMatch
{
    public long Id { get; set; }
    public long PlayerId { get; set; }
    public long MatchId { get; set; }
    public int Elo { get; set; }
    public int? Set1 { get; set; }
    public int? Set2 { get; set; }
    public int? Set3 { get; set; }
    public int? Set4 { get; set; }
    public int? Set5 { get; set; }
    public int? TieBreak { get; set; }
    public WinnerResult WinnerResult { get; set; }
    public int? MatchPoint { get; set; }
    public int? WonSets { get; set; }
    public int? WonGames { get; set; }
    public int? GrantedElo { get; set; }
    public virtual Player Player { get; set; } = null!;
    public virtual Match Match { get; set; } = null!;
}
