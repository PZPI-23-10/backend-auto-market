using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IVehicleBrandRepository : IBaseRepository<VehicleBrand>
{
    Task<bool> Exists(string name);
}