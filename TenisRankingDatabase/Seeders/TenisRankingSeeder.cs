using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TenisRankingDatabase.Enums;

namespace TenisRankingDatabase.Seeders;

public class TenisRankingSeeder
{
    private readonly TenisRankingDbContext _dbContext;
    private readonly SettingsSeeder _settingsSeeder;
    private readonly PlayersSeeder _playersSeeder;
    public TenisRankingSeeder(TenisRankingDbContext dbContext, SettingsSeeder settingsSeeder, PlayersSeeder playersSeeder)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _settingsSeeder = settingsSeeder ?? throw new ArgumentNullException(nameof(settingsSeeder));
        _playersSeeder = playersSeeder ?? throw new ArgumentNullException(nameof(playersSeeder));
    }

    public void Seed()
    {
        if (!_dbContext.Database.CanConnect())
        {
            return;
        }

        if (!_dbContext.Settings.Any())
        {
            _settingsSeeder.Seed();
        }

        if (!_dbContext.Players.Any())
        {
            _playersSeeder.Seed();
        }

        var tournaments = _dbContext.Tournaments.Where(x => x.Ended && x.TournamentStatus != TournamentStatus.Ended).ToList();
        if (tournaments.Any())
        {
            foreach (var item in tournaments)
            {
                item.TournamentStatus = TournamentStatus.Ended;
            }
            _dbContext.Tournaments.UpdateRange(tournaments);
            _dbContext.SaveChanges();
        }

        UpdateDatabase();
    }

    private void UpdateDatabase()
    {
        using var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            if (_dbContext.Database.IsRelational())
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();
                if (pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate();
                }
            }

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
        }
    }
}
