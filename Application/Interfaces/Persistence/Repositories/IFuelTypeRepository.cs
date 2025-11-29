using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IFuelTypeRepository : IBaseRepository<FuelType>
{
    Task<bool> Exists(string name);
}