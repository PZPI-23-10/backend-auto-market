using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VehicleModelRepository(DataContext context)
    : BaseRepository<VehicleModel>(context), IVehicleModelRepository
{
    public async Task<IEnumerable<VehicleModel>> GetByBrandAndTypeAsync(int brandId, int vehicleTypeId)
    {
        return await DataContext.VehicleModels
            .Where(vm => vm.BrandId == brandId)
            .Where(vm => vm.VehicleTypeId == vehicleTypeId)
            .Include(vm => vm.Brand)
            .Include(vm => vm.VehicleType)
            .ToListAsync();
    }
}