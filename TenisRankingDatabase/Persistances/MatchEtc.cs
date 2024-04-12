using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenisRankingDatabase.Tables;

namespace TenisRankingDatabase.Persistances;

internal class MatchEtc : IEntityTypeConfiguration<Match>
{
    [Obsolete]
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        _ = builder.HasKey(x => x.Id);

        builder.HasOne(m => m.Tournament)
            .WithMany(t => t.Matches)
            .HasForeignKey(m => m.TournamentId);
    }
}
