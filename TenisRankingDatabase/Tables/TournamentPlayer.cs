using TenisRankingDatabase.Persistances;

namespace TenisRankingDatabase.Tables;

public class TournamentPlayer
{
    public long Id { get; set; }
    public long TournamentId { get; set; }
    public long PlayerId { get; set; }
    public int Place { get; set; }
    public bool Active { get; set; } = true;
    public virtual Tournament Tournament { get; set; } = null!;
    public virtual Player Player { get; set; } = null!;

    public int PlayedMatch => Player.PlayerMatches.Where(x => x.Match.TournamentId == TournamentId).Count();

    public string CalculateTournamentScore()
    {
        return (Player.PlayerMatches?
            .Where(x => x.Match?.TournamentId == TournamentId && x.MatchPoint != null)
            .Select(x => x.MatchPoint)
            .Sum() ?? 0)
            .ToString();
    }

    public int CalculateTournamentScoreInt()
    {
        return Player.PlayerMatches?
            .Where(x => x.Match?.TournamentId == TournamentId && x.MatchPoint != null)
            .Select(x => x.MatchPoint)
            .Sum() ?? 0;
    }

    public string CalculateWonSets()
    {
        return (Player.PlayerMatches?
            .Where(x => x.Match?.TournamentId == TournamentId && x.WonSets != null)
            .Select(x => x.WonSets)
            .Sum() ?? 0)
            .ToString();
    }

    public int CalculateWonSetsInt()
    {
        return Player.PlayerMatches?
            .Where(x => x.Match?.TournamentId == TournamentId && x.WonSets != null)
            .Select(x => x.WonSets)
            .Sum() ?? 0;
    }

    public string CalculateWonGems()
    {
        return (Player.PlayerMatches?
            .Where(x => x.Match?.TournamentId == TournamentId && x.MatchPoint != null)
            .Select(x => x.WonGames)
            .Sum() ?? 0).ToString();
    }

    public int CalculateWonGemsInt()
    {
        return Player.PlayerMatches?
            .Where(x => x.Match?.TournamentId == TournamentId && x.MatchPoint != null)
            .Select(x => x.WonGames)
            .Sum() ?? 0;
    }
}
