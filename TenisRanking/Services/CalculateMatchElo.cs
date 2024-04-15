using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using TenisRankingDatabase.Enums;
using System;
using Table = TenisRankingDatabase.Tables;
using TenisRankingDatabase;
using System.Linq;

namespace GameTools.Services;

public class CalculateMatchElo
{
    private readonly TenisRankingDbContext _dbContext;

    public CalculateMatchElo(TenisRankingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public bool CalculateAndSaveMatchElo(Table.Match match)
    {
        try
        {
            if (match.MatchWinnerResult == MatchWinnerResult.None)
            {
                return true;
            }
            var winPlayer = match.PlayerMatches.First(x => x.WinnerResult == WinnerResult.Win);
            var lostPlayer = match.PlayerMatches.First(x => x.WinnerResult == WinnerResult.Win);
            if (lostPlayer.Id == 1)
            {
                return true;
            }
            var elo = CalculateEloReceivedForMatch(winPlayer.Elo, lostPlayer.Elo);
            winPlayer.GrantedElo = elo.WinPlayerElo;
            lostPlayer.GrantedElo = elo.LostPlayerElo;
            _dbContext.Update(match);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private (int WinPlayerElo, int LostPlayerElo) CalculateEloReceivedForMatch(int winPlayerStartElo, int lostPlayerStartElo)
    {
        // Stała K - współczynnik kształtujący tempo zmiany rankingu
        // Im większe K, tym większe zmiany w rankingu po meczu
        // Można dostosować K według potrzeb lub standardów danego systemu rankingowego
        double K = 32;

        // Obliczanie oczekiwanej wygranej dla gracza, wykorzystując funkcję sigmoidalną
        double expectedWinForWinPlayer = 1 / (1 + Math.Pow(10, (lostPlayerStartElo - winPlayerStartElo) / 400.0));

        // Obliczanie oczekiwanej wygranej dla przegranego gracza
        double expectedWinForLostPlayer = 1 - expectedWinForWinPlayer;

        // Obliczanie zmiany w rankingu graczy
        int winPlayerEloChange = (int)(K * (1 - expectedWinForWinPlayer));
        int lostPlayerEloChange = (int)(K * (0 - expectedWinForLostPlayer));

        return (winPlayerEloChange, lostPlayerEloChange);
    }
}
