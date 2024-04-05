﻿namespace TenisRankingDatabase.Tables;

public class Setting
{
    public long Id { get; set; }
    public int StartElo { get; set; } = 1000;
    public bool AllMatches { get; set; } = false;
    public int NumberOfMatchesPerPlayer { get; set; } = 3;
    public int NumberOfSets { get; set; } = 2;
    public bool TieBreak { get; set; } = false;
    public bool ExtraPointsForTournamentWon { get; set; } = false;
}


