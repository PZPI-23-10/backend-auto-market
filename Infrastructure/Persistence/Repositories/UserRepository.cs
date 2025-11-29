using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(DataContext context) : IUserRepository
{
    public async Task<User?> GetUserByEmail(string email)
    {
        return await context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UserWithEmailExists(string email)
    {
        return await context.Set<User>().AnyAsync(u => u.Email == email);
    }

    public virtual async Task<User?> GetByIdAsync(int? id)
    {
        return await context.Set<User>().FindAsync(id);
    }

    public virtual async Task<IEnumerable<User>> GetAllAsync()
    {
        return await context.Set<User>().ToListAsync();
    }

    public virtual IQueryable<User> Query()
    {
        return context.Set<User>().AsQueryable();
    }

    public virtual async Task<User> AddAsync(User entity)
    {
        await context.Set<User>().AddAsync(entity);

        return entity;
    }

    public virtual User Update(User entity)
    {
        context.Set<User>().Update(entity);
        return entity;
    }

    public virtual void Remove(User entity)
    {
        context.Set<User>().Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<User> entities)
    {
        context.Set<User>().RemoveRange(entities);
    }
}