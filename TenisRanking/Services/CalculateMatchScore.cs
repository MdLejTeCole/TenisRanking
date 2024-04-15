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

    public bool CalculateAndSaveMatchScore(Table.Match match, MatchResult matchResult, MatchWinnerResult matchWinnerResult)
    {
        try
        {
            match.MatchResult = matchResult;
            match.MatchWinnerResult = matchWinnerResult;
            match.Confirmed = true;
            var points = CalculateMatchPointFirstPlayer(match, matchWinnerResult);
            match.PlayerMatches[0].MatchPoint = points.FirstPlayerScore;
            match.PlayerMatches[1].MatchPoint = points.SecondPlayerScore;
            match.PlayerMatches[0].WonSets = points.FirstPlayerSets;
            match.PlayerMatches[1].WonSets = points.SecondPlayerSets;
            var winnerResult = GetWinnerResult(matchWinnerResult);
            match.PlayerMatches[0].WinnerResult = winnerResult.Item1;
            match.PlayerMatches[1].WinnerResult = winnerResult.Item2;
            _dbContext.Matches.Update(match);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
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

    private (int FirstPlayerScore, int FirstPlayerSets, int SecondPlayerScore, int SecondPlayerSets) CalculateMatchPointFirstPlayer(Table.Match match, MatchWinnerResult matchWinnerResult)
    {
        var firstPlayer = 0;
        var secondPlayer = 0;
        if (matchWinnerResult == MatchWinnerResult.None)
        {
            return (firstPlayer, 0, secondPlayer, 0);
        }
        var sets = new List<bool?>
        {
            FirstPlayerWonSet(match.PlayerMatches[0].Set1, match.PlayerMatches[1].Set1),
            FirstPlayerWonSet(match.PlayerMatches[0].Set2, match.PlayerMatches[1].Set2),
            FirstPlayerWonSet(match.PlayerMatches[0].Set3, match.PlayerMatches[1].Set3),
            FirstPlayerWonSet(match.PlayerMatches[0].Set4, match.PlayerMatches[1].Set4),
            FirstPlayerWonSet(match.PlayerMatches[0].Set5, match.PlayerMatches[1].Set5)
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
        return (firstPlayer, setsFirstPlayer, secondPlayer, setsSecondPlayer);
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
