using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenisRankingDatabase.Tables;

namespace TenisRankingDatabase.Persistances;

internal class MetchEtc : IEntityTypeConfiguration<Match>
{
    [Obsolete]
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        _ = builder.HasKey(x => x.Id);
    }
}

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
