using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using TenisRankingDatabase.Enums;
using Table = TenisRankingDatabase.Tables;

namespace GameTools.Models;

public class MatchDto
{
    public long Id { get; set; }
    public MatchResult MatchResult { get; set; }
    public MatchWinnerResult MatchWinnerResult { get; set; }
    public bool Confirmed { get; set; }
    public PlayerMatchDto Player1 { get; set; }
    public PlayerMatchDto Player2 { get; set; }

    public static MatchDto Create(Table.Match match)
    {
        var firstPlayer = match.PlayerMatches.OrderBy(x => x.Id).First();
        var secondPlayer = match.PlayerMatches.OrderBy(x => x.Id).Last();
        return new MatchDto()
        {
            Id = match.Id,
            MatchResult = match.MatchResult,
            MatchWinnerResult = match.MatchWinnerResult,
            Confirmed = match.Confirmed,
            Player1 = new PlayerMatchDto()
            {
                Id = firstPlayer.Player.Id,
                FirstName = firstPlayer.Player.FirstName,
                LastName = firstPlayer.Player.LastName,
                Nick = firstPlayer.Player.Nick,
                Set1 = firstPlayer.Set1,
                Set2 = firstPlayer.Set2,
                Set3 = firstPlayer.Set3,
                Set4 = firstPlayer.Set4,
                Set5 = firstPlayer.Set5,
                WinnerResult = firstPlayer.WinnerResult,
            },
            Player2 = new PlayerMatchDto()
            {
                Id = secondPlayer.Player.Id,
                FirstName = secondPlayer.Player.FirstName,
                LastName = secondPlayer.Player.LastName,
                Nick = secondPlayer.Player.Nick,
                Set1 = secondPlayer.Set1,
                Set2 = secondPlayer.Set2,
                Set3 = secondPlayer.Set3,
                Set4 = secondPlayer.Set4,
                Set5 = secondPlayer.Set5,
                WinnerResult = secondPlayer.WinnerResult,
            }
        };
    }
}
