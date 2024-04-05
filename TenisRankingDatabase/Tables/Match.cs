using TenisRankingDatabase.Enums;

namespace TenisRankingDatabase.Tables;

public class Match
{
    public long Id { get; set; }
    public long TournamentId { get; set; }
    public MatchResult MatchResult { get; set; }
    public virtual Tournament Tournament { get; set; } = null!;
}

