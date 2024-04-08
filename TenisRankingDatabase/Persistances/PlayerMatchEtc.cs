using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenisRankingDatabase.Tables;

namespace TenisRankingDatabase.Persistances;

internal class PlayerMatchEtc : IEntityTypeConfiguration<PlayerMatch>
{
    [Obsolete]
    public void Configure(EntityTypeBuilder<PlayerMatch> builder)
    {
        _ = builder.HasKey(x => x.Id);
        _ = builder.HasIndex(bt => new { bt.PlayerId, bt.MatchId })
            .IsUnique();
    }
}
