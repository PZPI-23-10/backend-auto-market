using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class VehicleListingRepository(DataContext context) :
    BaseRepository<VehicleListing>(context),
    IVehicleListingRepository { }