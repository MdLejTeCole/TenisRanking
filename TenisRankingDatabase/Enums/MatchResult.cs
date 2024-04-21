namespace TenisRankingDatabase.Enums;

public enum MatchResult
{
    Unsolved,
    Finished,
    FinishedBeforeEndWithADraw,
    Retired, //kontuzja zawodnika (Krecz)
    Disqualification,
    Walkover,
    JudgesDecision //decyzja sedziow
}

public enum TournamentStatus
{
    Started,
    Ended,
    Cancelled
}
