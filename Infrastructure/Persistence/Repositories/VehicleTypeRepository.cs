using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VehicleTypeRepository(DataContext context)
    : BaseRepository<VehicleType>(context), IVehicleTypeRepository
{
    public async Task<bool> Exists(string name)
    {
        return await DataContext.Set<VehicleType>().AnyAsync(b => b.Name == name);
    }
    public async Task<bool> ExistsById(int id)
    {
        return await DataContext.Set<VehicleType>().AnyAsync(b => b.Id == id);
    }
}