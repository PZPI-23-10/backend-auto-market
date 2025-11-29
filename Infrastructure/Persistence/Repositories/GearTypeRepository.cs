using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class GearTypeRepository(DataContext context)
    : BaseRepository<GearType>(context), IGearTypeRepository
{
    public async Task<bool> Exists(string name)
    {
        return await DataContext.Set<GearType>().AnyAsync(b => b.Name == name);
    }
}