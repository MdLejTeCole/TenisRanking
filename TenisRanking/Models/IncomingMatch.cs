namespace GameTools.Models;

public class IncomingMatch : CompletedMatch
{
    public int Player1Elo { get; set; }
    public int Player2Elo { get; set; }
}
