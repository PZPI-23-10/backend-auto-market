using Application.DTOs.Listings;

namespace Application.Interfaces.Services;

public interface IListingService
{
    public Task<int> CreateAndPublish(int userId, PublishedVehicleListingCommand dto);
    public Task UpdatePublished(int userId, int listingId, DraftVehicleListingCommand command);

    public Task<int> CreateDraft(int userId, DraftVehicleListingCommand dto);
    public Task UpdateDraft(int userId, int listingId, DraftVehicleListingCommand dto);
    public Task PublishDraft(int userId, int listingId, PublishedVehicleListingCommand command);

    public Task DeleteListing(int userId, int listingId);

    public Task<IEnumerable<VehicleListingResponse>> GetPublishedListings();
    public Task<IEnumerable<VehicleListingResponse>> GetUserListings(int userId);
    public Task<VehicleListingResponse> GetListingById(int id);
}