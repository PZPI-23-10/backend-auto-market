using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VehicleBrandRepository(DataContext context)
    : BaseRepository<VehicleBrand>(context), IVehicleBrandRepository
{
    public async Task<bool> Exists(string name)
    {
        return await DataContext.Set<VehicleBrand>().AnyAsync(x => x.Name == name);
    }
}