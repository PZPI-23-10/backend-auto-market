using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetUserByEmail(string email);
    Task<bool> UserWithEmailExists(string email);
}