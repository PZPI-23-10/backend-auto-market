using Application.Interfaces.Persistence;

namespace Infrastructure.Persistence;

public class UnitOfWork(DataContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}