using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int? id);
    Task<IQueryable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    T Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}