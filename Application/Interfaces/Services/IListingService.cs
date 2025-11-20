using Application.DTOs.Listings;

namespace Application.Interfaces.Services;

public interface IListingService
{
    Task<int> CreateAndPublish(int userId, PublishedVehicleListingCommand dto);
    Task UpdatePublished(int userId, int listingId, DraftVehicleListingCommand command);

    Task<int> CreateDraft(int userId, DraftVehicleListingCommand dto);
    Task UpdateDraft(int userId, int listingId, DraftVehicleListingCommand dto);
    Task PublishDraft(int userId, int listingId, PublishedVehicleListingCommand command);
    Task DeleteListing(int userId, int listingId);

    Task<IEnumerable<VehicleListingResponse>> GetPublishedListings(VehicleListingFilter? filter = null);
    Task<IEnumerable<VehicleListingResponse>> GetUserListings(int userId);
    Task<VehicleListingResponse> GetListingById(int id);
}