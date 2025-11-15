using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class VehiclePhotoRepository(DataContext context)
    : BaseRepository<VehiclePhoto>(context), IVehiclePhotoRepository { }