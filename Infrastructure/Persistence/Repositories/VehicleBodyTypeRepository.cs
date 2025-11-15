using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class VehicleBodyTypeRepository(DataContext context)
    : BaseRepository<VehicleBodyType>(context), IVehicleBodyTypeRepository { }