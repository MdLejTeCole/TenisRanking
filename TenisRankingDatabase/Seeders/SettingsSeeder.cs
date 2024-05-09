using TenisRankingDatabase.Tables;

namespace TenisRankingDatabase.Seeders;

public class SettingsSeeder
{
    private readonly TenisRankingDbContext _dbContext;

    public SettingsSeeder(TenisRankingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public void Seed()
    {
        _dbContext.Settings.Add(new Setting());
        _dbContext.SaveChanges();
    }
}
