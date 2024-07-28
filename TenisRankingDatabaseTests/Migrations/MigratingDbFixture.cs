using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TenisRankingDatabase;

namespace ItBuildsXUnitTests.Migrations;

public class MigratingDbFixture : IDisposable
{
    private bool disposedValue;
    private TenisRankingDbContext _itBuildsDbContext;
    private readonly DbContextOptions<TenisRankingDbContext> _itBuildsDbContextOptions;
    private static bool _deleteDb = true;

    public MigratingDbFixture()
    {
        if (_deleteDb == true) 
        {
            DeleteDb();
            _deleteDb = false;
        }
        _itBuildsDbContextOptions = new DbContextOptionsBuilder<TenisRankingDbContext>()
           .UseSqlite("Data Source=TenisRankingTest.db")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

        _itBuildsDbContext = new TenisRankingDbContext(_itBuildsDbContextOptions);
    }

    public async Task SetMigration(string migrationName)
    {
        await DatabaseContext.GetService<IMigrator>().MigrateAsync(migrationName);
    }

    public void RunScript(string fileLocation)
    {
        var script = File.ReadAllText(fileLocation);
        _itBuildsDbContext.Database.ExecuteSqlRaw(script);
    }
    public TenisRankingDbContext DatabaseContext => _itBuildsDbContext;

    private void DeleteDb()
    {
        try
        {
            if (File.Exists("TenisRankingTest.db"))
            {
                File.Delete("TenisRankingTest.db");
            }
        }
        catch (Exception)
        {

        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
