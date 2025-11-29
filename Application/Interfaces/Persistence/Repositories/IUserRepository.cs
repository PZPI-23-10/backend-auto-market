using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByEmail(string email);
    Task<bool> UserWithEmailExists(string email);
    Task<User?> GetByIdAsync(int? id);
    Task<IEnumerable<User>> GetAllAsync();
    IQueryable<User> Query();
    Task<User> AddAsync(User entity);
    User Update(User entity);
    void Remove(User entity);
    void RemoveRange(IEnumerable<User> entities);
}