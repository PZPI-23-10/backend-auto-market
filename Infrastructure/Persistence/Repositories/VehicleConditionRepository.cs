using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class VehicleConditionRepository(DataContext context)
    : BaseRepository<VehicleCondition>(context), IVehicleConditionRepository { }