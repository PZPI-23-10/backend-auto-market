using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IGearTypeRepository : IBaseRepository<GearType>
{
    Task<bool> Exists(string name);
}