using backend_auto_market.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_auto_market.Persistence;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;

        foreach (var changedEntity in ChangeTracker.Entries())
        {
            if (changedEntity.Entity is BaseAuditableEntity entity)
            {
                switch (changedEntity.State)
                {
                    case EntityState.Added:
                        entity.Created = now;
                        entity.LastModified = now;
                        break;

                    case EntityState.Modified:
                        Entry(entity).Property(x => x.Id).IsModified = false;
                        entity.LastModified = now;
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}