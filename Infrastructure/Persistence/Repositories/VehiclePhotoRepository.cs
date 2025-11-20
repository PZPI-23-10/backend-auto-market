using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VehiclePhotoRepository(DataContext context)
    : BaseRepository<VehiclePhoto>(context), IVehiclePhotoRepository
{
    public async Task<IEnumerable<VehiclePhoto>> GetByHash(string hash)
    {
        return await DataContext.Set<VehiclePhoto>().Where(x => x.Hash == hash).ToListAsync();
    }
}