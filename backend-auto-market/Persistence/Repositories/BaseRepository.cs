using backend_auto_market.Persistence.Models;

namespace backend_auto_market.Persistence.Repositories;

public abstract class BaseRepository<T>(DataContext context) where T : BaseEntity
{
    protected DataContext DataContext => context;

    public virtual async Task<T?> GetByIdAsync(int? id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public virtual async Task<IQueryable<T>> GetAllAsync()
    {
        return await Task.FromResult(context.Set<T>().AsQueryable());
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);

        return entity;
    }

    public virtual T Update(T entity)
    {
        context.Set<T>().Update(entity);
        return entity;
    }

    public virtual void Remove(T entity)
    {
        context.Set<T>().Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        context.Set<T>().RemoveRange(entities);
    }
}