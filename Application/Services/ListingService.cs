using Application.DTOs.Listings;
using Application.Interfaces.Services;

namespace Application.Services;

public class ListingService : IListingService
{
    public async Task CreateListing(CreateVehicleListingRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateListing(UpdateListingRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteListing(int listingId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<VehicleListingDto>> GetListings()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<VehicleListingDto>> GetUserListings(int userId)
    {
        throw new NotImplementedException();
    }
}