namespace GameTools.Models;

public class DoubleIncomingMatch : DoubleCompletedMatch
{
    public int Team1Player1Elo { get; set; }
    public int Team1Player2Elo { get; set; }
    public int Team2Player1Elo { get; set; }
    public int Team2Player2Elo { get; set; }
}
