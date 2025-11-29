using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IVehicleBodyTypeRepository : IBaseRepository<VehicleBodyType>
{
    Task<bool> ExistsById(int id);
}