using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IVehiclePhotoRepository : IBaseRepository<VehiclePhoto>
{
    Task<IEnumerable<VehiclePhoto>> GetByHash(string hash);
}