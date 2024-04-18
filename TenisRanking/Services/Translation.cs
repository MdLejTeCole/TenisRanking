using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
