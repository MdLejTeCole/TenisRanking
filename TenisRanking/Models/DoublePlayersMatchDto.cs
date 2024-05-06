using TenisRankingDatabase.Enums;

namespace GameTools.Models;

public class DoublePlayersMatchDto
{
    public long Player1Id { get; set; }
    public string Player1FirstName { get; set; } = string.Empty;
    public string Player1LastName { get; set; } = string.Empty;
    public string Player1Nick { get; set; } = string.Empty;
    public long Player2Id { get; set; }
    public string Player2FirstName { get; set; } = string.Empty;
    public string Player2LastName { get; set; } = string.Empty;
    public string Player2Nick { get; set; } = string.Empty;
    public int Set1 { get; set; }
    public int Set2 { get; set; }
    public int Set3 { get; set; }
    public int Set4 { get; set; }
    public int Set5 { get; set; }
    public WinnerResult WinnerResult { get; set; }
}
