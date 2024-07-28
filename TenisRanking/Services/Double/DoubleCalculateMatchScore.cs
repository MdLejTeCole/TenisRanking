using GameTools.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TenisRankingDatabase;
using TenisRankingDatabase.Enums;
using TenisRankingDatabase.Tables;

namespace GameTools.Services.Double;

public class DoubleCalculateMatchScore
{
    private readonly TenisRankingDbContext _dbContext;

    public DoubleCalculateMatchScore(TenisRankingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public bool CalculateAndSaveMatchScore(MixDoubleMatchDto matchDto, MatchResult matchResult, MatchWinnerResult matchWinnerResult)
    {
        try
        {
            var match = _dbContext.Matches
                .Include(x => x.PlayerMatches)
                    .ThenInclude(x => x.Player)
                .First(x => x.Id == matchDto.Id);
            var team1Player1 = match.PlayerMatches.First(x => x.PlayerId == matchDto.Team1.Player1Id);
            var team1Player2 = match.PlayerMatches.FirstOrDefault(x => x.PlayerId == matchDto.Team1.Player2Id);
            var team2Player1 = match.PlayerMatches.First(x => x.PlayerId == matchDto.Team2.Player1Id);
            var team2Player2 = match.PlayerMatches.FirstOrDefault(x => x.PlayerId == matchDto.Team2.Player2Id);
            match.MatchResult = matchResult;
            match.MatchWinnerResult = matchWinnerResult;
            match.Confirmed = true;
            var points = CalculateMatchPointFirstPlayer(matchDto.Team1, matchDto.Team2, matchWinnerResult);
            var winnerResult = GetWinnerResult(matchWinnerResult);
            UpdatePlayer(team1Player1, matchDto.Team1, points.FirstPlayerScore, points.FirstPlayerSets, points.FirstPlayerGames, winnerResult.Item1);
            UpdatePlayer(team1Player2, matchDto.Team1, points.FirstPlayerScore, points.FirstPlayerSets, points.FirstPlayerGames, winnerResult.Item1);
            UpdatePlayer(team2Player1, matchDto.Team2, points.SecondPlayerScore, points.SecondPlayerSets, points.SecondPlayerGames, winnerResult.Item2);
            UpdatePlayer(team2Player2, matchDto.Team2, points.SecondPlayerScore, points.SecondPlayerSets, points.SecondPlayerGames, winnerResult.Item2);
            _dbContext.Matches.Update(match);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void UpdatePlayer(PlayerMatch playerMatch, DoublePlayersMatchDto matchDto, int matchPoint, int wonSets, int wonGames, WinnerResult winnerResult)
    {
        if (playerMatch == null)
        {
            return;
        }
        playerMatch.Set1 = matchDto.Set1;
        playerMatch.Set2 = matchDto.Set2;
        playerMatch.Set3 = matchDto.Set3;
        playerMatch.Set4 = matchDto.Set4;
        playerMatch.Set5 = matchDto.Set5;
        playerMatch.MatchPoint = matchPoint;
        playerMatch.WonSets = wonSets;
        playerMatch.WonGames = wonGames;
        playerMatch.WinnerResult = winnerResult;
    }

    private (WinnerResult, WinnerResult) GetWinnerResult(MatchWinnerResult matchWinnerResult)
    {
        var firstPlayerMatchResult = WinnerResult.None;
        var secondPlayerMatchResult = WinnerResult.None;
        if (matchWinnerResult == MatchWinnerResult.FirstPlayerWin)
        {
            firstPlayerMatchResult = WinnerResult.Win;
            secondPlayerMatchResult = WinnerResult.Lost;
        }
        else if (matchWinnerResult == MatchWinnerResult.SecondPlayerWin)
        {
            firstPlayerMatchResult = WinnerResult.Lost;
            secondPlayerMatchResult = WinnerResult.Win;
        }
        return (firstPlayerMatchResult, secondPlayerMatchResult);
    }

    private (int FirstPlayerScore, int FirstPlayerSets, int FirstPlayerGames, int SecondPlayerScore, int SecondPlayerSets, int SecondPlayerGames) CalculateMatchPointFirstPlayer(DoublePlayersMatchDto team1, DoublePlayersMatchDto team2, MatchWinnerResult matchWinnerResult)
    {
        var firstPlayer = 0;
        var secondPlayer = 0;
        if (matchWinnerResult == MatchWinnerResult.None)
        {
            return (firstPlayer, 0, 0, secondPlayer, 0, 0);
        }
        var sets = new List<bool?>
        {
            FirstPlayerWonSet(team1.Set1, team2.Set1),
            FirstPlayerWonSet(team1.Set2, team2.Set2),
            FirstPlayerWonSet(team1.Set3, team2.Set3),
            FirstPlayerWonSet(team1.Set4, team2.Set4),
            FirstPlayerWonSet(team1.Set5, team2.Set5)
        };

        if (matchWinnerResult == MatchWinnerResult.FirstPlayerWin && sets.Any(x => x == false))
        {
            firstPlayer = 2;
            secondPlayer = 1;
        }
        else if (matchWinnerResult == MatchWinnerResult.SecondPlayerWin && sets.Any(x => x == true))
        {
            firstPlayer = 1;
            secondPlayer = 2;
        }
        else if (matchWinnerResult == MatchWinnerResult.FirstPlayerWin)
        {
            firstPlayer = 3;
            secondPlayer = 0;
        }
        else
        {
            firstPlayer = 0;
            secondPlayer = 3;
        }
        var setsFirstPlayer = sets.Where(x => x == true).Count();
        var setsSecondPlayer = sets.Where(x => x == false).Count();
        var gamesFirstPlayer = team1.Set1 + team1.Set2 + team1.Set3 + team1.Set4 + team1.Set5;
        var gamesSecondPlayer = team2.Set1 + team2.Set2 + team2.Set3 + team2.Set4 + team2.Set5;
        return (firstPlayer, setsFirstPlayer, gamesFirstPlayer, secondPlayer, setsSecondPlayer, gamesSecondPlayer);
    }

    private bool? FirstPlayerWonSet(int? setFirstPlayer, int? setSecondPlayer)
    {
        if (setFirstPlayer != null && setSecondPlayer != null)
        {
            if (setFirstPlayer > setSecondPlayer)
            {
                return true;
            }
            else if (setFirstPlayer < setSecondPlayer)
            {
                return false;
            }
        }
        return null;
    }
}
