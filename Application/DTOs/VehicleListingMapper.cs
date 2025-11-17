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
            BrandName = entity.ModelId != null
                ? new VehicleBrandResponse { Id = entity.Model!.BrandId, Name = entity.Model.Brand.Name }
                : null,
            ModelName = entity.ModelId != null
                ? new VehicleModelResponse { Id = entity.Model!.Id, Name = entity.Model.Name }
                : null,
            BodyTypeName = entity.BodyTypeId != null
                ? new VehicleBodyTypeResponse { Id = entity.BodyType!.Id, Name = entity.BodyType.Name }
                : null,
            ConditionName = entity.ConditionId != null
                ? new VehicleConditionResponse { Id = entity.Condition!.Id, Name = entity.Condition.Name }
                : null,
            CityName =
                entity.CityId != null ? new CityResponse { Id = entity.City!.Id, Name = entity.City.Name } : null,
            RegionName = entity.City != null
                ? new RegionResponse { Id = entity.City!.Region.Id, Name = entity.City!.Region.Name }
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