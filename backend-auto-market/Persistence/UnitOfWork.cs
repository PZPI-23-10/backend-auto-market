using backend_auto_market.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace backend_auto_market.Persistence;

public class UnitOfWork(DataContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        await context.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        IEnumerable<EntityEntry<BaseAuditableEntity>> auditableEntities = context
            .ChangeTracker
            .Entries<BaseAuditableEntity>();

        foreach (EntityEntry<BaseAuditableEntity> entity in auditableEntities)
        {
            DateTime now = DateTime.UtcNow;

            switch (entity.State)
            {
                case EntityState.Added:
                    entity.Property(e => e.Created).CurrentValue = now;
                    entity.Property(e => e.LastModified).CurrentValue = now;
                    break;

                case EntityState.Modified:
                    entity.Property(e => e.LastModified).CurrentValue = now;
                    break;
            }
        }
    }
}