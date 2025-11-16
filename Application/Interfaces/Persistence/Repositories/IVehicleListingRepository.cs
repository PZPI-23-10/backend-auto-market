using Application.DTOs.Listings;
using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IVehicleListingRepository : IBaseRepository<VehicleListing>
{
    Task<IEnumerable<VehicleListingResponse>> GetPublishedListingsAsync();
    Task<IEnumerable<VehicleListingResponse>> GetUserListingsAsync(int userId);
}