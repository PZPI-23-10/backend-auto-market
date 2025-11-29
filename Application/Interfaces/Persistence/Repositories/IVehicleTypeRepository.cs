using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IVehicleTypeRepository : IBaseRepository<VehicleType>
{ 
    Task<bool> Exists(string name);
    Task<bool> ExistsById(int id);
}