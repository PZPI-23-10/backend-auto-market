using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VehicleConditionRepository(DataContext context)
    : BaseRepository<VehicleCondition>(context), IVehicleConditionRepository
{
    public async Task<bool> Exists(string name)
    {
        return await DataContext.Set<VehicleCondition>().AnyAsync(x => x.Name == name);
    }
}