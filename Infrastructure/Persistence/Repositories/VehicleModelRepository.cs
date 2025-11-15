using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class VehicleModelRepository(DataContext context)
    : BaseRepository<VehicleModel>(context), IVehicleModelRepository { }