using Application.DTOs.Listings;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VehicleListingRepository(DataContext context)
    : BaseRepository<VehicleListing>(context), IVehicleListingRepository
{
    public async Task<List<VehicleListing>> GetPublishedListingsAsync(VehicleListingFilter? filter = null)
    {
        IQueryable<VehicleListing> query = DataContext.VehicleListings
            .Where(l => l.IsPublished);

        if (filter != null)
        {
            if (filter.VehicleTypeId.HasValue)
                query = query.Where(v => v.Model != null && v.Model.VehicleTypeId == filter.VehicleTypeId.Value);

            if (filter.BrandId.HasValue)
                query = query.Where(v => v.Model != null && v.Model.BrandId == filter.BrandId.Value);

            if (filter.ModelId.HasValue)
                query = query.Where(v => v.ModelId == filter.ModelId.Value);

            if (filter.BodyTypeId.HasValue)
                query = query.Where(v => v.BodyTypeId == filter.BodyTypeId.Value);

            if (filter.GearTypeId.HasValue)
                query = query.Where(v => v.GearTypeId == filter.GearTypeId.Value);

            if (filter.ConditionId.HasValue)
                query = query.Where(v => v.ConditionId == filter.ConditionId.Value);

            if (filter.FuelTypeId.HasValue)
                query = query.Where(v => v.FuelTypeId == filter.FuelTypeId.Value);

            if (filter.CityId.HasValue)
                query = query.Where(v => v.CityId == filter.CityId.Value);

            if (filter.YearFrom.HasValue)
                query = query.Where(v => v.Year >= filter.YearFrom.Value);

            if (filter.YearTo.HasValue)
                query = query.Where(v => v.Year <= filter.YearTo.Value);

            if (filter.MileageFrom.HasValue)
                query = query.Where(v => v.Mileage >= filter.MileageFrom.Value);

            if (filter.MileageTo.HasValue)
                query = query.Where(v => v.Mileage <= filter.MileageTo.Value);

            if (filter.PriceFrom.HasValue)
                query = query.Where(v => v.Price >= filter.PriceFrom.Value);

            if (filter.PriceTo.HasValue)
                query = query.Where(v => v.Price <= filter.PriceTo.Value);

            if (filter.HasAccident.HasValue)
                query = query.Where(v => v.HasAccident == filter.HasAccident.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<List<VehicleListing>> GetUserListingsAsync(int userId)
    {
        var listings = await DataContext.VehicleListings
            .Where(l => l.UserId == userId)
            .ToListAsync();

        return listings;
    }

    public async Task<bool> IsBodyTypeValidForModel(int modelId, int bodyTypeId)
    {
        return await context.VehicleModelBodyTypes
            .AnyAsync(x => x.VehicleModelId == modelId && x.BodyTypeId == bodyTypeId);
    }
}