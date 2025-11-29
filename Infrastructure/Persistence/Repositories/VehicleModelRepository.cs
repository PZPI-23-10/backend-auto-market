using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VehicleModelRepository(DataContext context)
    : BaseRepository<VehicleModel>(context), IVehicleModelRepository
{
    public async Task<IEnumerable<VehicleModel>> GetByBrandAndTypeAsync(int? brandId, int? vehicleTypeId, int? vehicleModelId)
    {
        var query = Query();

        if (vehicleModelId.HasValue)
        {
            query = query.Where(l => l.Id == vehicleModelId.Value);
        }
        
        if (brandId.HasValue)
        {
            query = query.Where(l => l.BrandId == brandId);
        }
        
        if (vehicleTypeId.HasValue)
        {
            query = query.Where(l => l.VehicleTypeId == vehicleTypeId);
        }

        query = query
            .Include(l => l.Brand)
            .Include(l => l.VehicleType);
            
            return await query.ToListAsync();
    }

    public async Task<bool> Exists(string name)
    {
        return await DataContext.Set<VehicleModel>().AnyAsync(l => l.Name == name);
    }
}