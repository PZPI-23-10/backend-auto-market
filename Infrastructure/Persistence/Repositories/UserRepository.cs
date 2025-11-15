using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(DataContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetUserByEmail(string email)
    {
        return await DataContext.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UserWithEmailExists(string email)
    {
        return await DataContext.Set<User>().AnyAsync(u => u.Email == email);
    }
}