using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TenisRankingDatabase;
using TenisRankingDatabase.Enums;

namespace GameTools.Services;

public class CalculateMatchElo
{
    private readonly TenisRankingDbContext _dbContext;

    public CalculateMatchElo(TenisRankingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public bool CalculateAndSaveMatchElo(long matchId, bool isDouble)
    {
        try
        {
            var match = _dbContext.Matches
                .Include(x => x.PlayerMatches)
                    .ThenInclude(x => x.Player)
                .First(x => x.Id == matchId);
            if (match.MatchWinnerResult == MatchWinnerResult.None)
            {
                return true;
            }
            var playerMatches = match.PlayerMatches.OrderBy(x => x.Id);
            var winPlayer = playerMatches.First(x => x.WinnerResult == WinnerResult.Win);
            var lostPlayer = playerMatches.First(x => x.WinnerResult == WinnerResult.Lost);
            if (lostPlayer.Id == 1)
            {
                return true;
            }
            (int WinPlayerElo, int LostPlayerElo) elo;
            if (match.PlayerMatches.Count() == 2)
            {
                elo = CalculateEloReceivedForMatch(winPlayer.Player.Elo, lostPlayer.Player.Elo);
            }
            else
            {
                var winPlayer2 = playerMatches.Last(x => x.WinnerResult == WinnerResult.Win);
                var lostPlayer2 = playerMatches.Last(x => x.WinnerResult == WinnerResult.Lost);
                elo = CalculateEloReceivedForMatch((winPlayer.Player.MixedDoubleElo + winPlayer2.Player.MixedDoubleElo) / 2, (lostPlayer.Player.MixedDoubleElo + lostPlayer2.Player.MixedDoubleElo) / 2);
                winPlayer2.Elo = winPlayer2.Player.MixedDoubleElo;
                lostPlayer2.Elo = lostPlayer2.Player.MixedDoubleElo;
                winPlayer2.GrantedElo = elo.WinPlayerElo;
                lostPlayer2.GrantedElo = elo.LostPlayerElo;
                winPlayer2.Player.MixedDoubleElo += elo.WinPlayerElo;
                lostPlayer2.Player.MixedDoubleElo += elo.LostPlayerElo;
            }

            winPlayer.GrantedElo = elo.WinPlayerElo;
            lostPlayer.GrantedElo = elo.LostPlayerElo;
            if (isDouble)
            {
                winPlayer.Elo = winPlayer.Player.MixedDoubleElo;
                lostPlayer.Elo = lostPlayer.Player.MixedDoubleElo;
                winPlayer.Player.MixedDoubleElo += elo.WinPlayerElo;
                lostPlayer.Player.MixedDoubleElo += elo.LostPlayerElo;
            }
            else
            {
                winPlayer.Elo = winPlayer.Player.Elo;
                lostPlayer.Elo = lostPlayer.Player.Elo;
                winPlayer.Player.Elo += elo.WinPlayerElo;
                lostPlayer.Player.Elo += elo.LostPlayerElo;
            }

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
