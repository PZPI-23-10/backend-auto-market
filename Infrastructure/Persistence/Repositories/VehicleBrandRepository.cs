using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class VehicleBrandRepository(DataContext context)
    : BaseRepository<VehicleBrand>(context), IVehicleBrandRepository { }