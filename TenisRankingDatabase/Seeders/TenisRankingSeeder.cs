using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace TenisRankingDatabase.Seeders;

public class TenisRankingSeeder
{
    private readonly TenisRankingDbContext _dbContext;
    public TenisRankingSeeder(TenisRankingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public void Seed()
    {
        if (!_dbContext.Database.CanConnect())
        {
            return;
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
        catch (Exception e)
        {
            transaction.Rollback();
        }
    }
}
