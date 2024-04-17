using TenisRankingDatabase.Tables;

namespace TenisRankingDatabase.Seeders;

public class PlayersSeeder
{
    private readonly TenisRankingDbContext _dbContext;

    public PlayersSeeder(TenisRankingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public void Seed()
    {
        _dbContext.Players.Add(new Player()
        {
            FirstName = "Brak",
            LastName = "Przeciwnika",
            Elo = 0,
            Active = false,
        });
        _dbContext.SaveChanges();
    }
}