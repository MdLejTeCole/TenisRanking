using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TenisRankingDatabase;
using TenisRankingDatabase.Enums;
using TenisRankingDatabase.Tables;

namespace GameTools.Services;

public class CalculateAfterEndTournament
{
    private readonly TenisRankingDbContext _dbContext;
    private readonly CalculateMatchElo _calculateMatchElo;

    public CalculateAfterEndTournament(TenisRankingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _calculateMatchElo = new CalculateMatchElo(dbContext);
    }

    public bool CalculateAndSaveUpdatesForTournament(long tournamentId)
    {
        try
        {
            UpdateElo(tournamentId);
            UpdateScore(tournamentId);
            var tournament = _dbContext.Tournaments.First(x => x.Id == tournamentId);
            tournament.Ended = true;
            _dbContext.Update(tournament);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    private void UpdateScore(long tournamentId)
    {
        var matches = _dbContext.PlayerMatches
            .Include(x => x.Match)
            .Where(x => x.Match.TournamentId == tournamentId);
        var players = _dbContext.TournamentPlayers
            .Include(x => x.Player)
            .Where(x => x.TournamentId == tournamentId);
        foreach (var player in players )
        {
            var playerMatches = matches.Where(x => x.PlayerId == player.PlayerId);
            player.Player.WinTournaments += 0; //todo
            player.Player.TournamentsPoints += playerMatches.Select(x => x.MatchPoint).Sum() ?? 0;
            player.Player.TournamentsPlayed += 1;
            player.Player.WinMatches += playerMatches.Where(x => x.WinnerResult == WinnerResult.Win).Count();
            player.Player.LostMatches += playerMatches.Where(x => x.WinnerResult == WinnerResult.Lost).Count();
            _dbContext.Players.Update(player.Player);
        }
        _dbContext.SaveChanges();
    }

    public void UpdateElo(long tournamentId)
    {
        var matchIds = _dbContext.Matches
            .Include(x => x.PlayerMatches)
            .Where(x => x.TournamentId == tournamentId).OrderBy(x => x.Round)
            .Select(x => x.Id);

        foreach (var matchId in matchIds)
        {
            _calculateMatchElo.CalculateAndSaveMatchElo(matchId);
        }
    }
}
