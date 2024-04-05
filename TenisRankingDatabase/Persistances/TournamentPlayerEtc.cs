using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenisRankingDatabase.Tables;

namespace TenisRankingDatabase.Persistances;

internal class TournamentPlayerEtc : IEntityTypeConfiguration<TournamentPlayer>
{
    public void Configure(EntityTypeBuilder<TournamentPlayer> builder)
    {
        _ = builder.HasKey(x => x.Id);
        _ = builder.HasIndex(bt => new { bt.TournamentId, bt.PlayerId })
            .IsUnique();
    }
}
