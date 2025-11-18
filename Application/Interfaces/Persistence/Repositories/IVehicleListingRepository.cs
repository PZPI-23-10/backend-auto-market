using Application.DTOs.Listings;
using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IVehicleListingRepository : IBaseRepository<VehicleListing>
{
    Task<List<VehicleListing>> GetPublishedListingsAsync(VehicleListingFilter? filter = null);
    Task<List<VehicleListing>> GetUserListingsAsync(int userId);
}