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

    public void CalculateAndSaveMatchScore(Table.Match match, MatchResult matchResult, MatchWinnerResult matchWinnerResult)
    {
        match.MatchResult = matchResult;
        match.MatchWinnerResult = matchWinnerResult;
        match.Confirmed = true;
        var points = CalculateMatchPointFirstPlayer(match, matchWinnerResult);
        match.PlayerMatches[0].MatchPoint = points.Item1;
        match.PlayerMatches[1].MatchPoint = points.Item2;
        var winnerResult = GetWinnerResult(matchWinnerResult);
        match.PlayerMatches[0].WinnerResult = winnerResult.Item1;
        match.PlayerMatches[1].WinnerResult = winnerResult.Item2;
        match.Tournament = null;
        try
        {
            _dbContext.Matches.Update(match);
            _dbContext.SaveChanges();
        }
        catch (Exception e)
        {


        }
    }

    private (WinnerResult, WinnerResult) GetWinnerResult(MatchWinnerResult matchWinnerResult)
    {
        var firstPlayerMatchResult = WinnerResult.None;
        var secondPlayerMatchResult = WinnerResult.None;
        if (matchWinnerResult == MatchWinnerResult.Draw)
        {
            firstPlayerMatchResult = WinnerResult.Draw;
            secondPlayerMatchResult = WinnerResult.Draw;
        }
        else if (matchWinnerResult == MatchWinnerResult.FirstPlayerWin)
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

    private (int, int) CalculateMatchPointFirstPlayer(Table.Match match, MatchWinnerResult matchWinnerResult)
    {
        var firstPlayer = 0;
        var secondPlayer = 0;
        if (matchWinnerResult == MatchWinnerResult.None || matchWinnerResult == MatchWinnerResult.Draw)
        {
            return (firstPlayer, secondPlayer);
        }

        var set1 = FirstPlayerWonSet(match.PlayerMatches[0].Set1, match.PlayerMatches[1].Set1);
        var set2 = FirstPlayerWonSet(match.PlayerMatches[0].Set2, match.PlayerMatches[1].Set2);
        var set3 = FirstPlayerWonSet(match.PlayerMatches[0].Set3, match.PlayerMatches[1].Set3);
        var set4 = FirstPlayerWonSet(match.PlayerMatches[0].Set4, match.PlayerMatches[1].Set4);
        var set5 = FirstPlayerWonSet(match.PlayerMatches[0].Set5, match.PlayerMatches[1].Set5);
        if (matchWinnerResult == MatchWinnerResult.FirstPlayerWin && (set1 == false || set2 == false || set3 == false || set4 == false || set5 == false))
        {
            firstPlayer = 2;
            secondPlayer = 1;
        }
        else if (matchWinnerResult == MatchWinnerResult.SecondPlayerWin && (set1 == true || set2 == true || set3 == true || set4 == true || set5 == true))
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
        return (firstPlayer, secondPlayer);
    }

    private bool? FirstPlayerWonSet(int? setFirstPlayer, int? setSecondPlayer)
    {
        if (setFirstPlayer != null && setSecondPlayer != null)
        {
            if (setFirstPlayer > setSecondPlayer)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return null;
    }
}
