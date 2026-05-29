using Application.DTOs.Listings;
using Application.DTOs.Location;
using Application.DTOs.Vehicle;
using Domain.Entities;

namespace Application.DTOs;

public static class VehicleListingMapper
{
    public static VehicleListingResponse ToResponseDto(VehicleListing entity)
    {
        VehicleModel? model = entity.Model;
        VehicleBrand? brand = model?.Brand;
        VehicleType? vehicleType = model?.VehicleType;
        VehicleBodyType? bodyType = entity.BodyType;
        VehicleCondition? condition = entity.Condition;
        City? city = entity.City;
        Region? region = city?.Region;
        GearType? gearType = entity.GearType;
        FuelType? fuelType = entity.FuelType;

        return new VehicleListingResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Brand = brand != null
                ? new VehicleBrandResponse { Id = brand.Id, Name = brand.Name }
                : null,
            Model = model != null
                ? new VehicleModelResponse { Id = model.Id, Name = model.Name }
                : null,
            BodyType = bodyType != null
                ? new VehicleBodyTypeResponse { Id = bodyType.Id, Name = bodyType.Name }
                : null,
            Condition = condition != null
                ? new VehicleConditionResponse { Id = condition.Id, Name = condition.Name }
                : null,
            City = city != null
                ? new CityResponse { Id = city.Id, Name = city.Name }
                : null,
            Region = region != null
                ? new RegionResponse { Id = region.Id, Name = region.Name }
                : null,
            GearType = gearType != null
                ? new GearTypeResponse { Id = gearType.Id, Name = gearType.Name }
                : null,
            FuelType = fuelType != null
                ? new FuelTypeResponse { Id = fuelType.Id, Name = fuelType.Name }
                : null,
            VehicleType = vehicleType != null
                ? new VehicleTypeResponse { Id = vehicleType.Id, Name = vehicleType.Name }
                : null,

            Year = entity.Year,
            Description = entity.Description,
            Mileage = entity.Mileage,
            HasAccident = entity.HasAccident ?? false,
            ColorHex = entity.ColorHex,
            Price = entity.Price ?? 0,
            Number = entity.Number ?? "",
            IsPublished = entity.IsPublished,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            Vin = entity.Vin,
            IsVerified = entity.IsVerified,
            PhotoUrls = entity.Photos.Select(p => new PhotoResponse
                { Id = p.Id, Url = p.PhotoUrl, SortOrder = p.SortOrder }).ToArray(),
            CreatedAt = entity.Created.DateTime
        };
    }
}
