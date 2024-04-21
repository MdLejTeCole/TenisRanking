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
            var transaction = _dbContext.Database.BeginTransaction();
            UpdateElo(tournamentId);
            UpdateScore(tournamentId);
            var tournament = _dbContext.Tournaments.First(x => x.Id == tournamentId);
            tournament.TournamentStatus = TournamentStatus.Ended;
            _dbContext.Update(tournament);
            _dbContext.SaveChanges();
            transaction.Commit();
            return true;
        }
        catch (Exception)
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
                .ThenInclude(x => x.PlayerMatches)
            .Where(x => x.TournamentId == tournamentId);
        var tournament = _dbContext.Tournaments.First(x => x.Id == tournamentId);
        int place = 1;
        foreach (var player in players.ToList().OrderByDescending(x => x.CalculateTournamentScoreInt()).ThenByDescending(x => x.CalculateWonSets()).ThenByDescending(x => x.CalculateWonGems()))
        {
            var playerMatches = matches.Where(x => x.PlayerId == player.PlayerId);
            if (tournament.ExtraPointsForTournamentWon)
            {
                player.Player.Elo += place == 1 ? tournament.ExtraPoints1Place : 0;
                player.Player.Elo += place == 2 ? tournament.ExtraPoints2Place : 0;
                player.Player.Elo += place == 3 ? tournament.ExtraPoints3Place : 0;
            }
            player.Place = place;
            player.Player.Tournament1Place += place == 1 ? 1 : 0;
            player.Player.Tournament2Place += place == 2 ? 1 : 0;
            player.Player.Tournament3Place += place == 3 ? 1 : 0;
            player.Player.TournamentsPoints += player.CalculateTournamentScoreInt();
            player.Player.TournamentsPlayed += 1;
            player.Player.WinMatches += playerMatches.Where(x => x.WinnerResult == WinnerResult.Win).Count();
            player.Player.LostMatches += playerMatches.Where(x => x.WinnerResult == WinnerResult.Lost).Count();
            _dbContext.Players.Update(player.Player);
            place++;
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
