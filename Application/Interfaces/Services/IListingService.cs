using Application.DTOs.Listings;

namespace Application.Interfaces.Services;

public interface IListingService
{
    public Task CreateListing(CreateVehicleListingRequest request);

    public Task UpdateListing(UpdateListingRequest request);

    public Task DeleteListing(int listingId);

    public Task<IEnumerable<VehicleListingDto>> GetListings();
    public Task<IEnumerable<VehicleListingDto>> GetUserListings(int userId);
}