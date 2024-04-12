﻿using System;
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

public class IncomingMatch : CompletedMatch
{
    public int Player1Elo { get; set; }
    public int Player2Elo { get; set; }
}