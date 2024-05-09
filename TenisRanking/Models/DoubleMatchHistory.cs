using GameTools.Services;
using System;
using System.Linq;
using TenisRankingDatabase.Enums;
using TenisRankingDatabase.Tables;
using Table = TenisRankingDatabase.Tables;

namespace GameTools.Models;

public class DoubleMatchHistory
{
    public long Id { get; set; }
    public string TournmentName { get; set; }
    public DateOnly TournmentDate { get; set; }
    public string MatchResult { get; set; }
    public string Names { get; set; }
    public string WinnerResults { get; set; }
    public string GrantedElos { get; set; }
    public string Elos { get; set; }
    public string Set1 { get; set; }
    public string Set2 { get; set; }
    public string Set3 { get; set; }
    public string Set4 { get; set; }
    public string Set5 { get; set; }
    public string MatchPoint { get; set; }
    public string WonSets { get; set; }
    public string WonGames { get; set; }

    public static DoubleMatchHistory Create(Table.Match match)
    {
        PlayerMatch team1Player1;
        PlayerMatch team2Player1;
        string names;
        string elos;
        if (match.MatchWinnerResult == MatchWinnerResult.None)
        {
            team1Player1 = match.PlayerMatches[0];
            team2Player1 = match.PlayerMatches[1];
            names = $"{team1Player1.Player.FirstName} {team1Player1.Player.LastName} ({team1Player1.Player.FirstName})\n" +
                $"vs\n" +
                $"{team2Player1.Player.FirstName} {team2Player1.Player.LastName} ({team2Player1.Player.FirstName})";
            elos = $"{team1Player1.Elo}\n\n{team2Player1.Elo}";
        }
        else
        {
            team1Player1 = match.PlayerMatches.First(x => x.WinnerResult == WinnerResult.Win);
            team2Player1 = match.PlayerMatches.First(x => x.WinnerResult == WinnerResult.Lost);
            if (match.MatchResult != TenisRankingDatabase.Enums.MatchResult.NoOpponent)
            {
                var team1Player2 = match.PlayerMatches.Last(x => x.WinnerResult == WinnerResult.Win);
                var team2Player2 = match.PlayerMatches.Last(x => x.WinnerResult == WinnerResult.Win);
                names = $"{team1Player1.Player.FirstName} {team1Player1.Player.LastName} ({team1Player1.Player.FirstName})\n" +
                $"{team1Player2.Player.FirstName} {team1Player2.Player.LastName} ({team1Player2.Player.Nick})\n" +
                $"vs\n" +
                $"{team2Player1.Player.FirstName} {team2Player1.Player.LastName} ({team2Player1.Player.FirstName})\n" +
                $"{team2Player2.Player.FirstName} {team2Player2.Player.LastName} ({team2Player2.Player.Nick})";
                elos = $"{team1Player1.Elo}\n{team2Player1.Elo}\n\n{team2Player1.Elo}\n{team2Player2.Elo}";
            }
            else
            {
                names = $"{team1Player1.Player.FirstName} {team1Player1.Player.LastName} ({team1Player1.Player.FirstName})\n" +
                $"vs\n" +
                $"{team2Player1.Player.FirstName} {team2Player1.Player.LastName} ({team2Player1.Player.FirstName})";
                elos = $"{team1Player1.Elo}\n\n{team2Player1.Elo}";
            }
        }
        return new DoubleMatchHistory()
        {
            Id = match.Id,
            TournmentName = match.Tournament.Name,
            TournmentDate = match.Tournament.Date,
            MatchResult = Translation.MatchResultTranslation[match.MatchResult],
            Names = names,
            WinnerResults = $"{Translation.WinnerResultTranslation[team1Player1.WinnerResult]}\n\n{Translation.WinnerResultTranslation[team2Player1.WinnerResult]}",
            Elos = elos,
            GrantedElos = $"+{team1Player1.GrantedElo}\n {team2Player1.GrantedElo}",
            Set1 = $"{team1Player1.Set1}\n\n{team2Player1.Set1}",
            Set2 = $"{team1Player1.Set2}\n\n{team2Player1.Set2}",
            Set3 = $"{team1Player1.Set3}\n\n{team2Player1.Set3}",
            Set4 = $"{team1Player1.Set4}\n\n{team2Player1.Set4}",
            Set5 = $"{team1Player1.Set5}\n\n{team2Player1.Set5}",
            MatchPoint = $"{team1Player1.MatchPoint}\n\n{team2Player1.MatchPoint}",
            WonSets = $"{team1Player1.WonSets}\n\n{team2Player1.WonSets}",
            WonGames = $"{team1Player1.WonGames}\n\n{team2Player1.WonGames}",
        };
    }
}
