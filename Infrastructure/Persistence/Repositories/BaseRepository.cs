using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public abstract class BaseRepository<T>(DataContext context) : IBaseRepository<T> where T : BaseEntity
{
    protected DataContext DataContext => context;

    public virtual async Task<T?> GetByIdAsync(int? id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await context.Set<T>().ToListAsync();
    }

    public virtual IQueryable<T> Query()
    {
        return context.Set<T>().AsQueryable();
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