using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TenisRankingDatabase.Extensions;
using TenisRankingDatabase.Persistances;
using TenisRankingDatabase.Tables;

namespace TenisRankingDatabase;

public class TenisRankingDbContext : DbContext
{
    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<Setting> Settings { get; set; } = null!;
    public DbSet<Tournament> Tournaments { get; set; } = null!;
    public DbSet<TournamentPlayer> TournamentPlayers { get; set; } = null!;
    public DbSet<Match> Metches { get; set; } = null!;
    public DbSet<PlayerMatch> PlayerMatches { get; set; } = null!;

    public TenisRankingDbContext(DbContextOptions<TenisRankingDbContext> options) : base(options)
    {
        _ = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder ?? throw new ArgumentNullException(nameof(optionsBuilder));
        optionsBuilder.UseLoggerFactory(new LoggerFactory());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureEntity<Player, PlayerEtc>();
        modelBuilder.ConfigureEntity<Setting, SettingEtc>();
        modelBuilder.ConfigureEntity<Tournament, TournamentEtc>();
        modelBuilder.ConfigureEntity<TournamentPlayer, TournamentPlayerEtc>();
        modelBuilder.ConfigureEntity<Match, MetchEtc>();
        modelBuilder.ConfigureEntity<PlayerMatch, PlayerMatchEtc>();
    }
}
