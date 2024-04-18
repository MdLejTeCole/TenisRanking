using GameTools.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Table = TenisRankingDatabase.Tables;

namespace GameTools.Models;

public class TournamentHistory
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateOnly Date { get; set; }
    public int NumberOfSets { get; set; }
    public int AvarageElo { get; set; }
    public string ExtraPointsForTournamentWon { get; set; }
    public int ExtraPoints1Place { get; set; }
    public int ExtraPoints2Place { get; set; }
    public int ExtraPoints3Place { get; set; }
    public List<string> Players { get; set; }

    public static TournamentHistory Create(Table.Tournament tournament)
    {
        var players = tournament.TournamentPlayers.OrderBy(p => p.Place).Select(x => $"{x.Player.FirstName} {x.Player.LastName} ({x.Player.Nick})").ToList();
        int minLength = 20;
        var players20 = players.Concat(Enumerable.Range(0, minLength - players.Count).Select(_ => "")).ToList();
        return new TournamentHistory()
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Date = tournament.Date,
            NumberOfSets = tournament.NumberOfSets,
            AvarageElo = tournament.AvarageElo,
            ExtraPointsForTournamentWon = Translation.ExtraPointsTranslation[tournament.ExtraPointsForTournamentWon],
            ExtraPoints1Place = tournament.ExtraPoints1Place,
            ExtraPoints2Place = tournament.ExtraPoints2Place,
            ExtraPoints3Place = tournament.ExtraPoints3Place,
            Players = players20,
        };
    }
}
