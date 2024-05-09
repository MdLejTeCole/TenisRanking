using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TenisRankingDatabase;

//Potrzebne do tworzenia migracji
public class TenisRankingDbContextFactory : IDesignTimeDbContextFactory<TenisRankingDbContext>
{
    public TenisRankingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenisRankingDbContext>();

        return new TenisRankingDbContext(optionsBuilder.Options);
    }
}
