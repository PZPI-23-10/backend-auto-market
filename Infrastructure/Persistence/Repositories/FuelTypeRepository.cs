using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class FuelTypeRepository(DataContext context)
    : BaseRepository<FuelType>(context), IFuelTypeRepository
{
    public async Task<bool> Exists(string name)
    {
        return await DataContext.Set<FuelType>().AnyAsync(b => b.Name == name);
    }
}