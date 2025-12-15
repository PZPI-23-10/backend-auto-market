namespace Application.Interfaces.Services;

public interface IFavouriteService
{
    Task<bool> AddToFavouritesAsync(int userId, int vehicleListingId);
    Task<bool> RemoveFromFavouritesAsync(int userId, int vehicleListingId);
}