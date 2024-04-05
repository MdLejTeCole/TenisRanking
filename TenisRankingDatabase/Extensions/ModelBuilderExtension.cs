using Microsoft.EntityFrameworkCore;

namespace TenisRankingDatabase.Extensions;

public static class ModelBuilderExtension
{
    public static void ConfigureEntity<TEntity, TEntityEtc>(this ModelBuilder modelBuilder)
        where TEntity : class
        where TEntityEtc : class, IEntityTypeConfiguration<TEntity>, new() =>
        new TEntityEtc().Configure(modelBuilder.Entity<TEntity>());
}
