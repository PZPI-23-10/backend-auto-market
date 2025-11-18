using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int? id);
    Task<IEnumerable<T>> GetAllAsync();
    IQueryable<T> Query();
    Task<T> AddAsync(T entity);
    T Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}