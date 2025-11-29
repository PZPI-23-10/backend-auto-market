using Application.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Persistence;

public class UnitOfWork(DataContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        await context.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        IEnumerable<EntityEntry<IAuditableEntity>> auditableEntities = context
            .ChangeTracker
            .Entries<IAuditableEntity>();

        foreach (EntityEntry<IAuditableEntity> entity in auditableEntities)
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