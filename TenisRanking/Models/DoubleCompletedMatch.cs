namespace GameTools.Models;

public class DoubleCompletedMatch
{
    public long Team1Player1Id { get; set; }
    public long Team1Player2Id { get; set; }
    public long? Team2Player1Id { get; set; }
    public long? Team2Player2Id { get; set; }
}
