namespace Application.DTOs.Listings;

public class VehicleListingResponse
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string? BrandName { get; init; }
    public string? ModelName { get; init; }
    public string? BodyTypeName { get; init; }
    public string? ConditionName { get; init; }
    public string? CityName { get; init; }
    public string? RegionName { get; init; }
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