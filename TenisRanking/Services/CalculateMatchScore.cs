using GameTools.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TenisRankingDatabase;
using TenisRankingDatabase.Enums;
using TenisRankingDatabase.Tables;
using Table = TenisRankingDatabase.Tables;

namespace GameTools.Services;

public class CalculateMatchScore
{
    private readonly TenisRankingDbContext _dbContext;

    public CalculateMatchScore(TenisRankingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public (bool Result, bool NeedRefresh) CalculateAndSaveMatchScore(MatchDto matchDto, MatchResult matchResult, MatchWinnerResult matchWinnerResult)
    {
        try
        {
            var match = _dbContext.Matches
                .Include(x => x.PlayerMatches)
                    .ThenInclude(x => x.Player)
                .First(x => x.Id == matchDto.Id);
            var needRefresh = CheckNextMatchesWasGeneratedWithOtherWinner(matchDto, match, matchWinnerResult);
            var player1 = match.PlayerMatches.First(x => x.PlayerId == matchDto.Player1.Id);
            var player2 = match.PlayerMatches.First(x => x.PlayerId == matchDto.Player2.Id);
            match.MatchResult = matchResult;
            match.MatchWinnerResult = matchWinnerResult;
            player1.Set1 = matchDto.Player1.Set1;
            player1.Set2 = matchDto.Player1.Set2;
            player1.Set3 = matchDto.Player1.Set3;
            player1.Set4 = matchDto.Player1.Set4;
            player1.Set5 = matchDto.Player1.Set5;
            player2.Set1 = matchDto.Player2.Set1;
            player2.Set2 = matchDto.Player2.Set2;
            player2.Set3 = matchDto.Player2.Set3;
            player2.Set4 = matchDto.Player2.Set4;
            player2.Set5 = matchDto.Player2.Set5;
            match.Confirmed = true;
            var points = CalculateMatchPointFirstPlayer(player1, player2, matchWinnerResult);
            player1.MatchPoint = points.FirstPlayerScore;
            player2.MatchPoint = points.SecondPlayerScore;
            player1.WonSets = points.FirstPlayerSets;
            player2.WonSets = points.SecondPlayerSets;
            player1.WonGames = points.FirstPlayerGames;
            player2.WonGames = points.SecondPlayerGames;
            var winnerResult = GetWinnerResult(matchWinnerResult);
            player1.WinnerResult = winnerResult.Item1;
            player2.WinnerResult = winnerResult.Item2;
            _dbContext.Matches.Update(match);
            _dbContext.SaveChanges();
            return (true, needRefresh);
        }
        catch (Exception)
        {
            return (false, false);
        }
    }

    private bool CheckNextMatchesWasGeneratedWithOtherWinner(MatchDto matchDto, Table.Match match, MatchWinnerResult matchWinnerResult)
    {
        if (matchDto.Confirmed && 
            match.MatchWinnerResult is MatchWinnerResult.FirstPlayerWin or MatchWinnerResult.SecondPlayerWin &&
            match.MatchWinnerResult != matchWinnerResult)
        {
            var playersMatches = _dbContext.PlayerMatches
                .Include(x => x.Match)
                .Where(x => x.Match.TournamentId == match.TournamentId && x.Match.Round == match.Round + 1 && !x.Match.Confirmed && (x.PlayerId == matchDto.Player1.Id || x.PlayerId == matchDto.Player2.Id));
            if (playersMatches.Count() == 2)
            {
                var firstPlayerMatch = playersMatches.First(x => x.PlayerId == matchDto.Player1.Id);
                var secondPlayerMatch = playersMatches.First(x => x.PlayerId == matchDto.Player2.Id);
                firstPlayerMatch.PlayerId = matchDto.Player2.Id;
                secondPlayerMatch.PlayerId = matchDto.Player1.Id;
                return true;
            }
        }
        return false;
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

    private (int FirstPlayerScore, int FirstPlayerSets, int FirstPlayerGames, int SecondPlayerScore, int SecondPlayerSets, int SecondPlayerGames) CalculateMatchPointFirstPlayer(PlayerMatch player1, PlayerMatch player2, MatchWinnerResult matchWinnerResult)
    {
        var firstPlayer = 0;
        var secondPlayer = 0;
        if (matchWinnerResult == MatchWinnerResult.None)
        {
            return (firstPlayer, 0, 0, secondPlayer, 0, 0);
        }
        var sets = new List<bool?>
        {
            FirstPlayerWonSet(player1.Set1, player2.Set1),
            FirstPlayerWonSet(player1.Set2, player2.Set2),
            FirstPlayerWonSet(player1.Set3, player2.Set3),
            FirstPlayerWonSet(player1.Set4, player2.Set4),
            FirstPlayerWonSet(player1.Set5, player2.Set5)
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
        var gamesFirstPlayer = player1.Set1 + player1.Set2 + player1.Set3 + player1.Set4 + player1.Set5;
        var gamesSecondPlayer = player2.Set1 + player2.Set2 + player2.Set3 + player2.Set4 + player2.Set5;
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
