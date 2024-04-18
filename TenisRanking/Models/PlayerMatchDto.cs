using TenisRankingDatabase.Enums;

namespace GameTools.Models;

public class PlayerMatchDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Nick { get; set; } = string.Empty;
    public int Set1 { get; set; }
    public int Set2 { get; set; }
    public int Set3 { get; set; }
    public int Set4 { get; set; }
    public int Set5 { get; set; }
    public WinnerResult WinnerResult { get; set; }
}
