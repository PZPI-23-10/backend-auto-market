using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IVehicleModelRepository : IBaseRepository<VehicleModel>
{
    Task<IEnumerable<VehicleModel>> GetByBrandAndTypeAsync(int brandId, int vehicleTypeId);
}