using System.Linq;
using TenisRankingDatabase.Enums;
using Windows.Services.Maps.OfflineMaps;
using Table = TenisRankingDatabase.Tables;

namespace GameTools.Models;

public class MixDoubleMatchDto
{
    public long Id { get; set; }
    public MatchResult MatchResult { get; set; }
    public MatchWinnerResult MatchWinnerResult { get; set; }
    public bool Confirmed { get; set; }
    public DoublePlayersMatchDto Team1 { get; set; }
    public DoublePlayersMatchDto Team2 { get; set; }

    public static MixDoubleMatchDto Create(Table.Match match)
    {
        var players = match.PlayerMatches.OrderBy(x => x.Id).ToList();
        var dto = new MixDoubleMatchDto()
        {
            Id = match.Id,
            MatchResult = match.MatchResult,
            MatchWinnerResult = match.MatchWinnerResult,
            Confirmed = match.Confirmed,
        };
        if (players.Count == 4)
        {
            dto.Team1 = new DoublePlayersMatchDto()
            {
                Player1Id = players[0].Player.Id,
                Player1FirstName = players[0].Player.FirstName,
                Player1LastName = players[0].Player.LastName,
                Player1Nick = players[0].Player.Nick,
                Player2Id = players[1].Player.Id,
                Player2FirstName = players[1].Player.FirstName,
                Player2LastName = players[1].Player.LastName,
                Player2Nick = players[1].Player.Nick,
                Set1 = players[0].Set1,
                Set2 = players[0].Set2,
                Set3 = players[0].Set3,
                Set4 = players[0].Set4,
                Set5 = players[0].Set5,
                WinnerResult = players[0].WinnerResult,
            };
            dto.Team2 = new DoublePlayersMatchDto()
            {
                Player1Id = players[2].Player.Id,
                Player1FirstName = players[2].Player.FirstName,
                Player1LastName = players[2].Player.LastName,
                Player1Nick = players[2].Player.Nick,
                Player2Id = players[3].Player.Id,
                Player2FirstName = players[3].Player.FirstName,
                Player2LastName = players[3].Player.LastName,
                Player2Nick = players[3].Player.Nick,
                Set1 = players[2].Set1,
                Set2 = players[2].Set2,
                Set3 = players[2].Set3,
                Set4 = players[2].Set4,
                Set5 = players[2].Set5,
                WinnerResult = players[2].WinnerResult,
            };
        }
        else if (players.Count == 2)
        {
            dto.Team1 = new DoublePlayersMatchDto()
            {
                Player1Id = players[0].Player.Id,
                Player1FirstName = players[0].Player.FirstName,
                Player1LastName = players[0].Player.LastName,
                Player1Nick = players[0].Player.Nick,
                //Player2Id = players[0].Player.Id,
                //Player2FirstName = players[0].Player.FirstName,
                //Player2LastName = players[0].Player.LastName,
                //Player2Nick = players[0].Player.Nick,
                Set1 = players[0].Set1,
                Set2 = players[0].Set2,
                Set3 = players[0].Set3,
                Set4 = players[0].Set4,
                Set5 = players[0].Set5,
                WinnerResult = players[0].WinnerResult,
            };
            dto.Team2 = new DoublePlayersMatchDto()
            {
                Player1Id = players[1].Player.Id,
                Player1FirstName = players[1].Player.FirstName,
                Player1LastName = players[1].Player.LastName,
                Player1Nick = players[1].Player.Nick,
                //Player2Id = players[1].Player.Id,
                //Player2FirstName = players[1].Player.FirstName,
                //Player2LastName = players[1].Player.LastName,
                //Player2Nick = players[1].Player.Nick,
                Set1 = players[0].Set1,
                Set2 = players[0].Set2,
                Set3 = players[0].Set3,
                Set4 = players[0].Set4,
                Set5 = players[0].Set5,
                WinnerResult = players[0].WinnerResult,
            };
        }
        return dto;
    }
}
