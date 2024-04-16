using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        return new MatchDto()
        {
            Id = match.Id,
            MatchResult = match.MatchResult,
            MatchWinnerResult = match.MatchWinnerResult,
            Confirmed = match.Confirmed,
            Player1 = new PlayerMatchDto()
            {
                Id = match.PlayerMatches[0].Player.Id,
                FirstName = match.PlayerMatches[0].Player.FirstName,
                LastName = match.PlayerMatches[0].Player.LastName,
                Nick = match.PlayerMatches[0].Player.Nick,
                Set1 = match.PlayerMatches[0].Set1,
                Set2 = match.PlayerMatches[0].Set2,
                Set3 = match.PlayerMatches[0].Set3,
                Set4 = match.PlayerMatches[0].Set4,
                Set5 = match.PlayerMatches[0].Set5,
                WinnerResult = match.PlayerMatches[0].WinnerResult,
            },
            Player2 = new PlayerMatchDto()
            {
                Id = match.PlayerMatches[1].Player.Id,
                FirstName = match.PlayerMatches[1].Player.FirstName,
                LastName = match.PlayerMatches[1].Player.LastName,
                Nick = match.PlayerMatches[1].Player.Nick,
                Set1 = match.PlayerMatches[1].Set1,
                Set2 = match.PlayerMatches[1].Set2,
                Set3 = match.PlayerMatches[1].Set3,
                Set4 = match.PlayerMatches[1].Set4,
                Set5 = match.PlayerMatches[1].Set5,
                WinnerResult = match.PlayerMatches[1].WinnerResult,
            }
        };
    }

}

public class PlayerMatchDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Nick { get; set; } = string.Empty;
    public int? Set1 { get; set; }
    public int? Set2 { get; set; }
    public int? Set3 { get; set; }
    public int? Set4 { get; set; }
    public int? Set5 { get; set; }
    public WinnerResult WinnerResult { get; set; }
}

