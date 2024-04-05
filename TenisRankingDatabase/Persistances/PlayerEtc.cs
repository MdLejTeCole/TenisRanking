using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenisRankingDatabase.Tables;

namespace TenisRankingDatabase.Persistances;

internal class PlayerEtc : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        _ = builder.HasKey(x => x.Id);
    }
}
