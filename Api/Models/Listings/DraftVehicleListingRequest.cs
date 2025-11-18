namespace Api.Models.Listings;

public class DraftVehicleListingRequest
{
    public int? ModelId { get; set; }
    public int? BodyTypeId { get; set; }
    public int? GearTypeId { get; set; }
    public int? FuelTypeId { get; set; }
    public int? ConditionId { get; set; }
    public int? CityId { get; set; }
    public int? Year { get; set; }
    public int? Mileage { get; set; }
    public string? Number { get; set; }
    public string? ColorHex { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public bool? HasAccident { get; set; }

    public IFormFile[]? Photos { get; set; }
}