using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class VehicleTypeRepository(DataContext context)
    : BaseRepository<VehicleType>(context), IVehicleTypeRepository { }