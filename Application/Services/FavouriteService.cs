using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class FavouriteService() : IFavouriteService
{
    private readonly IDataContext _context;

    public async Task<bool> AddToFavouritesAsync(int userId, int vehicleListingId)
    {
        var exists = await _context.FavouriteVehicles
            .AnyAsync(f => f.UserId == userId && f.VehicleListingId == vehicleListingId);

        if (exists)
        {
            return false;
        }
        
        var favourite = new FavouriteVehicle
        {
            UserId = userId,
            VehicleListingId = vehicleListingId
        };

        await _context.FavouriteVehicles.AddAsync(favourite);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveFromFavouritesAsync(int userId, int vehicleListingId)
    {
        var favourite = await _context.FavouriteVehicles
            .FirstOrDefaultAsync(f => f.UserId == userId && f.VehicleListingId == vehicleListingId);

        if (favourite == null)
        {
            return false;
        }

        _context.FavouriteVehicles.Remove(favourite);
        await _context.SaveChangesAsync();

        return true;
    }
}
