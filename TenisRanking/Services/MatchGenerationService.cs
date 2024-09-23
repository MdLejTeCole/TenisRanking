using GameTools.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public List<long> GenerateFirstRound(long tournamentId)
    {
        var activePlayers = _dbContext.TournamentPlayers
            .Include(x => x.Player)
            .ThenInclude(x => x.PlayerMatches)
            .Where(x => x.TournamentId == tournamentId)
            .ToList();

        activePlayers = activePlayers.Where(x => x.Active).ToList();

        if (activePlayers.Count() < 2)
        {
            return new List<long>();
        }
        List<TournamentPlayer> sortedPlayers;
        if (activePlayers.Count % 2 == 1)
        {
            var sorted = activePlayers.OrderBy(x => Guid.NewGuid()).ToList();
            sortedPlayers = sorted.Take(activePlayers.Count - 1).ToList().Concat(new List<TournamentPlayer>() { sorted.Last() }).ToList();
        }
        else
        {
            sortedPlayers = activePlayers.OrderByDescending(x => x.Player.Elo).ThenBy(x => Guid.NewGuid()).ToList();
        }
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
                Player2Id = 1, //default
                Player2Elo = 0, //default
            });
        }
        return CreateMatches(tournamentId, 1, incomingMatches);
    }

    public List<long> GenerateNextRound(long tournamentId, int roundNumber)
    {
        var players = GetPlayerIdsInScoreOrder(tournamentId);
        if (players.Count() < 2)
        {
            return new List<long>();
        }
        var completedMatches = GetCompletedMatches(tournamentId);
        var availableMatches = GetAvailableMatches(players, completedMatches);
        var collectedAvaliableMatches =  FindAvaliableMatchCombinations(players, availableMatches);
        var incomingMatch = new List<IncomingMatch>();
        foreach (var item in collectedAvaliableMatches) //todo
        {
            incomingMatch.Add(new IncomingMatch()
            {
                Player1Id = item.Player1Id,
                Player2Id = item.Player2Id,
            });
        }
        return CreateMatches(tournamentId, roundNumber, incomingMatch);
    }

    private List<CompletedMatch> FindAvaliableMatchCombinations(List<long> playerIds, List<CompletedMatch> availableMatches)
    {
        var excludedAvailableMatches = new List<List<CompletedMatch>>();
        while (true)
        {
            var copiedPlayerIds = playerIds.ToList();
            var collectedAvaliableMatches = new List<CompletedMatch>();
            foreach (var item in availableMatches)
            {
                if (copiedPlayerIds.Any(x => x == item.Player1Id) && copiedPlayerIds.Any(x => x == item.Player2Id))
                {
                    collectedAvaliableMatches.Add(item);
                    if (excludedAvailableMatches.Any(x => x.SequenceEqual(collectedAvaliableMatches)))
                    {
                        collectedAvaliableMatches.Remove(item);
                        continue;
                    }
                    copiedPlayerIds.Remove(item.Player1Id);
                    copiedPlayerIds.Remove(item.Player2Id);
                }
                if (!copiedPlayerIds.Any())
                {
                    return collectedAvaliableMatches;
                }
            }
            if (!collectedAvaliableMatches.Any())
            {
                return new List<CompletedMatch>();
            }
            excludedAvailableMatches.Add(collectedAvaliableMatches);
        }
    }

    private List<long> GetPlayerIdsInScoreOrder(long tournamentId)
    {
        var playerPoints = _dbContext.TournamentPlayers
            .Include(x => x.Player)
                .ThenInclude(x => x.PlayerMatches)
                    .ThenInclude(x => x.Match)

            .Where(tp => tp.TournamentId == tournamentId && tp.Active)
            .Select(g => new 
            {
                g.Active,
                g.PlayerId,
                Points = g.CalculateTournamentScoreInt()
            })
            .ToList()
            .Where(x => x.Active)
            .OrderByDescending(x => x.Points)
            .ThenBy(x => Guid.NewGuid())
            .Select(x => x.PlayerId)
            .ToList();

        return playerPoints;
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
                Player2Id = match.PlayerMatches[1].PlayerId
            });
        }
        return completedMatches;
    }

    private List<CompletedMatch> GetAvailableMatches(List<long> activePlayers, List<CompletedMatch> completedMatches)
    {
        var avaliableMatches = new List<CompletedMatch>();
        if (activePlayers.Count() % 2 == 1)
        {
            activePlayers.Add(1); //default
        }
        foreach (var player1 in activePlayers)
        {
            foreach (var player2 in activePlayers)
            {
                if (player1 == player2)
                {
                    continue;
                }
                if (!completedMatches.Any(x => (x.Player1Id == player1 && x.Player2Id == player2) || (x.Player1Id == player2 && x.Player2Id == player1)) &&
                    !avaliableMatches.Any(x => (x.Player1Id == player1 && x.Player2Id == player2) || (x.Player1Id == player2 && x.Player2Id == player1)))
                {
                    avaliableMatches.Add(new CompletedMatch() { Player1Id = player1, Player2Id = player2});
                }
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
                        },
                    new PlayerMatch
                    {
                        PlayerId = (long)match.Player2Id,
                        Elo = (int)match.Player2Elo,
                    }
                };
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
