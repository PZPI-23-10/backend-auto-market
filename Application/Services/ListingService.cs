using Application.DTOs.Auth;
using Application.Interfaces.Services;
using CreateVehicleListingRequest = Application.DTOs.Listings.CreateVehicleListingRequest;

namespace Application.Services;

public class ListingService : IListingService
{
    Task<IEnumerable<VehicleListingResponse>> GetListingsAsync();
    Task<VehicleListingResponse?> GetListingByIdAsync(int id);
    Task<IEnumerable<VehicleListingResponse>> GetUserListingsAsync(int userId);
    Task<VehicleListingResponse> CreateListingAsync(CreateVehicleListingRequest request, int userId);
    Task UpdateListingAsync(int listingId, UpdateVehicleListingRequest request, int userId);
    Task DeleteListingAsync(int listingId, int userId);
}