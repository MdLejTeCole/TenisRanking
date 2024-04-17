using System;
using System.Linq;
using System.Text.RegularExpressions;
using TenisRankingDatabase.Enums;
using TenisRankingDatabase.Tables;
using Table = TenisRankingDatabase.Tables;

namespace GameTools.Models;

public class MatchHistory
{
    public long Id { get; set; }
    public string TournmentName { get; set; }
    public DateOnly TournmentDate { get; set; }
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

    public static MatchHistory Create(Table.Match match)
    {
        PlayerMatch player1;
        PlayerMatch player2;
        if (match.MatchWinnerResult == MatchWinnerResult.None)
        {
            player1 = match.PlayerMatches[0];
            player2 = match.PlayerMatches[1];
        }
        else
        {
            player1 = match.PlayerMatches.First(x => x.WinnerResult == WinnerResult.Win);
            player2 = match.PlayerMatches.First(x => x.WinnerResult == WinnerResult.Lost);
        }
        return new MatchHistory()
        {
            Id = match.Id,
            TournmentName = match.Tournament.Name,
            TournmentDate = match.Tournament.Date,
            Names = $"{player1.Player.FirstName} {player1.Player.LastName} ({player1.Player.FirstName})\n{player2.Player.Nick} {player2.Player.LastName} ({player2.Player.Nick})",
            WinnerResults = $"{player1.WinnerResult}\n{player2.WinnerResult}",
            Elos = $"{player1.Elo}\n{player2.Elo}",
            GrantedElos = $"+{player1.GrantedElo}\n {player2.GrantedElo}",
            Set1 = $"{player1.Set1}\n{player2.Set1}",
            Set2 = $"{player1.Set2}\n{player2.Set2}",
            Set3 = $"{player1.Set3}\n{player2.Set3}",
            Set4 = $"{player1.Set4}\n{player2.Set4}",
            Set5 = $"{player1.Set5}\n{player2.Set5}",
            MatchPoint = $"{player1.MatchPoint}\n{player2.MatchPoint}",
            WonSets = $"{player1.WonSets}\n{player2.WonSets}",
            WonGames = $"{player1.WonGames}\n{player2.WonGames}",
        };
    }
}
