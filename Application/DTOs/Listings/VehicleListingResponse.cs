using Application.DTOs.Location;
using Application.DTOs.Vehicle;
using Domain.Entities;

namespace Application.DTOs.Listings;

public class VehicleListingResponse
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public VehicleBrandResponse? BrandName { get; init; }
    public VehicleModelResponse? ModelName { get; init; }
    public VehicleBodyTypeResponse? BodyTypeName { get; init; }
    public VehicleConditionResponse? ConditionName { get; init; }
    public CityResponse? CityName { get; init; }
    public RegionResponse? RegionName { get; init; }
    public int? Year { get; init; }
    public string? Description { get; init; }
    public int? Mileage { get; init; }
    public bool HasAccident { get; init; }
    public string? ColorHex { get; init; }
    public decimal? Price { get; init; }
    public string? Number { get; init; }
    public bool IsPublished { get; init; }
    public IEnumerable<string> PhotoUrls { get; init; } = [];
    public DateTime CreatedAt { get; init; }
}