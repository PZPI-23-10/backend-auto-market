using Application.DTOs.Location;
using Application.DTOs.Vehicle;

namespace Application.DTOs.Listings;

public class VehicleListingResponse
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public VehicleTypeResponse? VehicleType { get; init; }
    public VehicleBrandResponse? Brand { get; init; }
    public VehicleModelResponse? Model { get; init; }
    public VehicleBodyTypeResponse? BodyType { get; init; }
    public VehicleConditionResponse? Condition { get; init; }
    public GearTypeResponse? GearType { get; init; }
    public FuelTypeResponse? FuelType { get; init; }
    public RegionResponse? Region { get; init; }
    public CityResponse? City { get; init; }
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