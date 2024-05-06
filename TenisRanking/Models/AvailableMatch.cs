using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenisRankingDatabase.Tables;

namespace GameTools.Models;

public class CompletedMatch
{
    public long Player1Id { get; set; }
    public long Player2Id { get; set; }
}

public class DoubleCompletedMatch
{
    public long Team1Player1Id { get; set; }
    public long Team1Player2Id { get; set; }
    public long? Team2Player1Id { get; set; }
    public long? Team2Player2Id { get; set; }
}

public class IncomingMatch : CompletedMatch
{
    public int Player1Elo { get; set; }
    public int Player2Elo { get; set; }
}

public class DoubleIncomingMatch : DoubleCompletedMatch
{
    public int Team1Player1Elo { get; set; }
    public int Team1Player2Elo { get; set; }
    public int Team2Player1Elo { get; set; }
    public int Team2Player2Elo { get; set; }
}
