using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenisRankingDatabase.Tables;

namespace TenisRankingDatabase.Persistances;

internal class TournamentEtc : IEntityTypeConfiguration<Tournament>
{
    public void Configure(EntityTypeBuilder<Tournament> builder)
    {
        _ = builder.HasKey(x => x.Id);
    }
}
