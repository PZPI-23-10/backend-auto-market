using Application.DTOs;
using Application.DTOs.Listings;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VehicleListingRepository(DataContext context)
    : BaseRepository<VehicleListing>(context), IVehicleListingRepository
{
    public async Task<IEnumerable<VehicleListingResponse>> GetPublishedListingsAsync()
    {
        var listings = await DataContext.VehicleListings
            .Where(l => l.IsPublished)
            .ToListAsync();

        return listings.Select(VehicleListingMapper.ToResponseDto);
    }

    public async Task<IEnumerable<VehicleListingResponse>> GetUserListingsAsync(int userId)
    {
        var listings = await DataContext.VehicleListings
            .Where(l => l.UserId == userId)
            .ToListAsync();

        return listings.Select(VehicleListingMapper.ToResponseDto);
    }
}