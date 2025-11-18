using Application.DTOs.Listings;
using Application.DTOs.Location;
using Application.DTOs.Vehicle;
using Domain.Entities;

namespace Application.DTOs;

public static class VehicleListingMapper
{
    public static VehicleListingResponse ToResponseDto(VehicleListing entity)
    {
        return new VehicleListingResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Brand = entity.ModelId != null
                ? new VehicleBrandResponse { Id = entity.Model!.BrandId, Name = entity.Model.Brand.Name }
                : null,
            Model = entity.ModelId != null
                ? new VehicleModelResponse { Id = entity.Model!.Id, Name = entity.Model.Name }
                : null,
            BodyType = entity.BodyTypeId != null
                ? new VehicleBodyTypeResponse { Id = entity.BodyType!.Id, Name = entity.BodyType.Name }
                : null,
            Condition = entity.ConditionId != null
                ? new VehicleConditionResponse { Id = entity.Condition!.Id, Name = entity.Condition.Name }
                : null,
            City =
                entity.CityId != null ? new CityResponse { Id = entity.City!.Id, Name = entity.City.Name } : null,
            Region = entity.City != null
                ? new RegionResponse { Id = entity.City!.Region.Id, Name = entity.City!.Region.Name }
                : null,
            GearType = entity.GearTypeId != null
                ? new GearTypeResponse { Id = entity.GearType!.Id, Name = entity.GearType.Name }
                : null,
            FuelType = entity.FuelTypeId != null
                ? new FuelTypeResponse() { Id = entity.FuelType!.Id, Name = entity.FuelType.Name }
                : null,
            VehicleType = entity.ModelId != null
                ? new VehicleTypeResponse { Id = entity.Model!.VehicleType.Id, Name = entity.Model.VehicleType.Name }
                : null,

            Year = entity.Year,
            Description = entity.Description,
            Mileage = entity.Mileage,
            HasAccident = entity.HasAccident ?? false,
            ColorHex = entity.ColorHex,
            Price = entity.Price ?? 0,
            Number = entity.Number ?? "",
            IsPublished = entity.IsPublished,
            PhotoUrls = entity.Photos.Select(p => p.PhotoUrl),
            CreatedAt = entity.Created.DateTime
        };
    }
}