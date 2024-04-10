using GameTools.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenisRankingDatabase;
using TenisRankingDatabase.Tables;

namespace GameTools.Services;

public class MatchGenerationService
{
    private readonly TenisRankingDbContext _dbContext;

    public MatchGenerationService(TenisRankingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public List<long> GenerateFirstRound(long tournamentId, List<long> activePlayerIds)
    {
        var activePlayers = _dbContext.TournamentPlayers
            .Include(x => x.Player)
            .Where(x => x.TournamentId == tournamentId)
            .ToList();

        activePlayers = activePlayers.Where(x => activePlayerIds.Any(y => y == x.PlayerId)).ToList();

        if (activePlayers.Count() < 2)
        {
            return new List<long>();
        }

        var sortedPlayers = activePlayers.OrderByDescending(x => x.Player.Elo).ToList();
        var incomingMatches = new List<IncomingMatch>();
        for (int i = 0; i < sortedPlayers.Count / 2; i++)
        {
            incomingMatches.Add(new IncomingMatch()
            {
                Player1Id = sortedPlayers[i].PlayerId,
                Player1Elo = sortedPlayers[i].Player.Elo,
                Player2Id = sortedPlayers[i + sortedPlayers.Count / 2].PlayerId,
                Player2Elo = sortedPlayers[i + sortedPlayers.Count / 2].Player.Elo
            });
        }
        if (sortedPlayers.Count % 2 == 1)
        {
            incomingMatches.Add(new IncomingMatch()
            {
                Player1Id = sortedPlayers.Last().PlayerId,
                Player1Elo = sortedPlayers.Last().Player.Elo,
            });
        }
        return CreateMatches(tournamentId, 0, incomingMatches);
    }

    public List<long> GenerateNextRound(long tournamentId, List<long> activePlayerIds)
    {
        if (activePlayerIds.Count() < 2)
        {
            return new List<long>();
        }

        var completedMatches = GetCompletedMatches(tournamentId);
        var availableatches = GetAvailableMatches(activePlayerIds, completedMatches);
        return new List<long>();
    }

    private List<CompletedMatch> GetCompletedMatches(long tournamentId)
    {
        var matches = _dbContext.Matches
            .Include(m => m.PlayerMatches)
            .Where(m => m.TournamentId == tournamentId);
        var completedMatches = new List<CompletedMatch>();
        foreach (var match in matches)
        {
            completedMatches.Add(new CompletedMatch()
            {
                Player1Id = match.PlayerMatches[0].PlayerId,
                Player2Id = match.PlayerMatches.Count > 1 ? match.PlayerMatches[1].PlayerId : null
            });
        }
        return completedMatches;
    }

    private List<AvailableMatch> GetAvailableMatches(List<long> activePlayers, List<CompletedMatch> completedMatches)
    {
        var avaliableMatches = new List<AvailableMatch>();
        foreach (var player1 in activePlayers)
        {
            foreach (var player2 in activePlayers)
            {
                if (player1 == player2)
                {
                    continue;
                }
                if (completedMatches.Any(x => (x.Player1Id == player1 && x.Player2Id == player2) || (x.Player1Id == player2 && x.Player2Id == player1)) &&
                    avaliableMatches.Any(x => (x.Player1Id == player1 && x.Player2Id == player2) || (x.Player1Id == player2 && x.Player2Id == player1)))
                {
                    avaliableMatches.Add(new AvailableMatch() { Player1Id = player1, Player2Id = player2});
                }
            }
            if (completedMatches.Any(x => x.Player1Id == player1 && x.Player2Id == null) &&
                    avaliableMatches.Any(x => x.Player1Id == player1 && x.Player2Id == null))
            {
                avaliableMatches.Add(new AvailableMatch() { Player1Id = player1, Player2Id = null });
            }
        }
        return avaliableMatches;
    }

    private List<long> CreateMatches(long tournamentId, int round, List<IncomingMatch> incomingMatches)
    {
        var matches = new List<Match>();
        try
        {
            foreach (var match in incomingMatches)
            {
                var playerMatches = new List<PlayerMatch>()
                {
                    new PlayerMatch
                        {
                            PlayerId = match.Player1Id,
                            Elo = match.Player1Elo,
                        }
                };
                if (match.Player2Id != null)
                {
                    playerMatches.Add(new PlayerMatch
                    {
                        PlayerId = (long)match.Player2Id,
                        Elo = (int)match.Player2Elo,
                    });
                }
                matches.Add(new Match()
                {
                    TournamentId = tournamentId,
                    Round = round,
                    PlayerMatches = playerMatches
                });
            }
            _dbContext.Matches.AddRange(matches);
            _dbContext.SaveChanges();
            return matches.Select(x => x.Id).OrderBy(x => x).ToList();
        }
        catch (Exception)
        {
            return new List<long>();
        }
    }
}
