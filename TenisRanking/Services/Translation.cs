using System.Collections.Generic;
using TenisRankingDatabase.Enums;

namespace GameTools.Services;

public class Translation
{
    public static Dictionary<MatchResult, string> MatchResultTranslation = new()
    {
        [MatchResult.Unsolved] = "Nierozstrzygnięty",
        [MatchResult.Finished] = "Zakończony",
        [MatchResult.FinishedBeforeEndWithADraw] = "Zakończony przed czasem",
        [MatchResult.Retired] = "Kontuzja",
        [MatchResult.Disqualification] = "Dyskfalifikacja",
        [MatchResult.Walkover] = "Poddanie",
        [MatchResult.JudgesDecision] = "Decyzja sędziów",
        [MatchResult.NoOpponent] = "Brak przeciwnika",
    };

    public static Dictionary<WinnerResult, string> WinnerResultTranslation = new()
    {
        [WinnerResult.None] = "Brak",
        [WinnerResult.Win] = "Wygrana",
        [WinnerResult.Lost] = "Przegrana",
    };

    public static Dictionary<bool, string> ExtraPointsTranslation = new()
    {
        [false] = "Wyłączona",
        [true] = "Włączona",
    };

    public static Dictionary<TenisMatchType, string> TenisMatchTypeTranslation = new()
    {
        [TenisMatchType.Single] = "Singiel",
        [TenisMatchType.Double] = "Debel",
        [TenisMatchType.MixedDouble] = "Debel mieszany",
    };
}
