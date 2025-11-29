using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VehicleBodyTypeRepository(DataContext context)
    : BaseRepository<VehicleBodyType>(context), IVehicleBodyTypeRepository
{
    public async Task<bool> ExistsById(int id)
    {
        return await DataContext.Set<VehicleBodyType>().AnyAsync(b => b.Id == id);
    }
}