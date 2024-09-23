using GameTools.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using TenisRankingDatabase;
using TenisRankingDatabase.Enums;
using TenisRankingDatabase.Tables;
using Table = TenisRankingDatabase.Tables;

namespace GameTools.Services.Double;

public class DoubleMatchGenerationService
{
    private readonly TenisRankingDbContext _dbContext;

    public DoubleMatchGenerationService(TenisRankingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public (List<long> Matches, List<long> Pauses) GenerateFirstRound(long tournamentId)
    {
        var activePlayers = _dbContext.TournamentPlayers
            .Include(x => x.Player)
            .ThenInclude(x => x.PlayerMatches)
            .Where(x => x.TournamentId == tournamentId)
            .ToList();

        activePlayers = activePlayers.Where(x => x.Active).ToList();

        if (activePlayers.Count() < 4)
        {
            return (new List<long>(), new List<long>());
        }

        var sortedPlayers = activePlayers.OrderByDescending(x => Guid.NewGuid()).ToList();
        var incomingMatches = new List<DoubleIncomingMatch>();
        for (int i = 0; i < sortedPlayers.Count / 4 * 2;)
        {
            incomingMatches.Add(new DoubleIncomingMatch()
            {
                Team1Player1Id = sortedPlayers[i].PlayerId,
                Team1Player1Elo = sortedPlayers[i].Player.MixedDoubleElo,
                Team1Player2Id = sortedPlayers[i + 1].PlayerId,
                Team1Player2Elo = sortedPlayers[i + 1].Player.MixedDoubleElo,
                Team2Player1Id = sortedPlayers[i + sortedPlayers.Count / 2].PlayerId,
                Team2Player1Elo = sortedPlayers[i + sortedPlayers.Count / 2].Player.MixedDoubleElo,
                Team2Player2Id = sortedPlayers[i + 1 + sortedPlayers.Count / 2].PlayerId,
                Team2Player2Elo = sortedPlayers[i + 1 + sortedPlayers.Count / 2].Player.MixedDoubleElo
            });
            i += 2;
        }
        foreach (var item in sortedPlayers.Skip(sortedPlayers.Count / 4 * 4))
        {
            incomingMatches.Add(new DoubleIncomingMatch()
            {
                Team1Player1Id = item.PlayerId,
                Team1Player1Elo = item.Player.MixedDoubleElo,
                Team1Player2Id = 1, //default
                Team1Player2Elo = 0
            });
        }
        return CreateMatches(tournamentId, 1, incomingMatches);
    }

    public (List<long> Matches, List<long> Pauses) GenerateNextRound(long tournamentId, int roundNumber)
    {
        var players = GetPlayerIdsInScoreOrder(tournamentId);
        if (players.Players.Count() < 4)
        {
            return (new List<long>(), new List<long>());
        }
        var completedMatches = GetCompletedMatches(tournamentId);
        var availableMatches = GetAvailableMatches(players.Players, completedMatches);
        var collectedAvaliableMatches = FindAvaliableMatchCombinations(players.Players, availableMatches);
        var incomingMatches = new List<DoubleIncomingMatch>();
        for (int i = 0; i < collectedAvaliableMatches.Count;)
        {
            incomingMatches.Add(new DoubleIncomingMatch()
            {
                Team1Player1Id = collectedAvaliableMatches[i].Player1Id,
                Team1Player2Id = collectedAvaliableMatches[i].Player2Id,
                Team2Player1Id = collectedAvaliableMatches[i + 1].Player1Id,
                Team2Player2Id = collectedAvaliableMatches[i + 1].Player2Id,
            });
            i += 2;
        }
        if (incomingMatches.Count == 0)
        {
            return (new List<long>(), new List<long>());
        }
        foreach (var item in players.Pauses)
        {
            incomingMatches.Add(new DoubleIncomingMatch()
            {
                Team1Player1Id = item,
                Team1Player2Id = 1, //default
            });
        }
        return CreateMatches(tournamentId, roundNumber, incomingMatches);
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

    public (List<long> Players, List<long> Pauses) GetPlayerIdsInScoreOrder(long tournamentId)
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
                Points = g.CalculateTournamentScoreInt(),
                MatchesCount = g.Player.PlayerMatches.Where(x => x.Match.MatchResult != MatchResult.NoOpponent && x.Match.TournamentId == tournamentId).Count()
            })
            .ToList()
            .Where(x => x.Active)
            .ToList();

        return (playerPoints
            .OrderBy(x => x.MatchesCount)
            .Take(playerPoints.Count / 4 * 4)
            .OrderByDescending(x => x.Points)
            .ThenBy(x => Guid.NewGuid())
            .Select(x => x.PlayerId)
            .ToList(), 
            playerPoints
            .OrderBy(x => x.MatchesCount)
            .Skip(playerPoints.Count / 4 * 4)
            .Select(x => x.PlayerId)
            .ToList());
    }

    private List<CompletedMatch> GetCompletedMatches(long tournamentId)
    {
        var matches = _dbContext.Matches
            .Include(m => m.PlayerMatches)
            .Where(m => m.TournamentId == tournamentId && m.MatchResult != MatchResult.NoOpponent);
        var completedMatches = new List<CompletedMatch>();

        foreach (var match in matches)
        {
            var playerMatches = match.PlayerMatches.OrderBy(x => x.Id).ToList();
            completedMatches.Add(new CompletedMatch()
            {
                Player1Id = playerMatches[0].PlayerId,
                Player2Id = playerMatches[1].PlayerId
            });
            if (playerMatches.Count == 4)
            {
                completedMatches.Add(new CompletedMatch()
                {
                    Player1Id = playerMatches[2].PlayerId,
                    Player2Id = playerMatches[3].PlayerId
                });
            }
        }
        return completedMatches;
    }

    private List<CompletedMatch> GetAvailableMatches(List<long> activePlayers, List<CompletedMatch> completedMatches)
    {
        var avaliableMatches = new List<CompletedMatch>();
        foreach (var player1 in activePlayers)
        {
            foreach (var player2 in activePlayers)
            {
                if (player1 == player2)
                {
                    continue;
                }
                if (!completedMatches.Any(x => x.Player1Id == player1 && x.Player2Id == player2 || x.Player1Id == player2 && x.Player2Id == player1) &&
                    !avaliableMatches.Any(x => x.Player1Id == player1 && x.Player2Id == player2 || x.Player1Id == player2 && x.Player2Id == player1))
                {
                    avaliableMatches.Add(new CompletedMatch() { Player1Id = player1, Player2Id = player2 });
                }
            }
        }
        return avaliableMatches;
    }

    private (List<long> Matches, List<long> Pauses) CreateMatches(long tournamentId, int round, List<DoubleIncomingMatch> incomingMatches)
    {
        var matches = new List<Table.Match>();
        try
        {
            foreach (var match in incomingMatches)
            {
                var playerMatches = new List<PlayerMatch>()
                {
                    new PlayerMatch
                    {
                        PlayerId = match.Team1Player1Id,
                        Elo = match.Team1Player1Elo,
                    },
                    new PlayerMatch
                    {
                        PlayerId = match.Team1Player2Id,
                        Elo = match.Team1Player2Elo,
                    },
                };
                if (match.Team2Player1Id is not null)
                {
                    playerMatches.AddRange(new List<PlayerMatch>()
                    {
                        new PlayerMatch
                        {
                            PlayerId = (long)match.Team2Player1Id,
                            Elo = match.Team2Player1Elo,
                        },
                        new PlayerMatch
                        {
                            PlayerId = (long)match.Team2Player2Id,
                            Elo = match.Team2Player2Elo,
                        }
                    });
                }
                matches.Add(new Table.Match()
                {
                    TournamentId = tournamentId,
                    Round = round,
                    PlayerMatches = playerMatches
                });
            }
            _dbContext.Matches.AddRange(matches);
            _dbContext.SaveChanges();
            return (matches.Select(x => x.Id).OrderBy(x => x).ToList(), matches.Where(x => x.PlayerMatches.Count() == 2).Select(x => x.Id).OrderBy(x => x).ToList());
        }
        catch (Exception)
        {
            return (new List<long>(), new List<long>());
        }
    }
}
