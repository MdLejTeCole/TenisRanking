namespace TenisRankingDatabase.Tables;

public class TournamentPlayer
{
    public long Id { get; set; }
    public long TournamentId { get; set; }
    public long PlayerId { get; set; }
    public bool Active { get; set; } = true;
    public virtual Tournament Tournament { get; set; } = null!;
    public virtual Player Player { get; set; } = null!;
}

