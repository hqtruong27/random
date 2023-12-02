using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Data;

public class StatisticsDbContext(DbContextOptions options) : IdentityDbContext<User, Role, string>(options)
{
    public DbSet<Spending> Spendings { get; set; } = default!;

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var trackedEntities = ChangeTracker.Entries<BaseTrackedEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

        var utcNow = DateTime.UtcNow;
        string emptyCreatedBy = string.Empty;

        foreach (var entityEntry in trackedEntities)
        {
            var entity = entityEntry.Entity;

            switch (entityEntry.State)
            {
                case EntityState.Added:
                    entity.Created = utcNow;
                    entity.CreatedBy = emptyCreatedBy;
                    break;
                case EntityState.Modified:
                    entity.Updated = utcNow;
                    entity.UpdatedBy = emptyCreatedBy;

                    if (entity.IsDeleted)
                    {
                        entity.DeletedAt = utcNow;
                    }
                    break;
                case EntityState.Deleted:
                    entity.IsDeleted = true;
                    entity.DeletedAt = utcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}