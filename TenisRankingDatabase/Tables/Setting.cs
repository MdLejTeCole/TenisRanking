﻿namespace TenisRankingDatabase.Tables;

public class Setting
{
    public long Id { get; set; }
    public int StartElo { get; set; } = 1000;
    public int StartMixedDoubleElo { get; set; } = 1000;
    public int NumberOfSets { get; set; } = 2;
    public bool TieBreak { get; set; } = false;
    public bool ExtraPointsForTournamentWon { get; set; } = false;
    public int ExtraPoints1Place { get; set; } = 0;
    public int ExtraPoints2Place { get; set; } = 0;
    public int ExtraPoints3Place { get; set; } = 0;
}
