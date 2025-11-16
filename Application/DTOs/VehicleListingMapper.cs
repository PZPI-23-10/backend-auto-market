using Application.DTOs.Listings;
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
            BrandName = entity.Model?.Brand?.Name,
            ModelName = entity.Model?.Name,
            BodyTypeName = entity.BodyType?.Name,
            ConditionName = entity.Condition?.Name,
            CityName = entity.City?.Name,
            RegionName = entity.City?.Region?.Name,
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