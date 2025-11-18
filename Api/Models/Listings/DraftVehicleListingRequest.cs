using System.ComponentModel.DataAnnotations;

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
    [Range(0, int.MaxValue)] public int? Mileage { get; set; }
    [Range(1900, 2100)] public string? Number { get; set; }

    [RegularExpression("^#([A-Fa-f0-9]{6})$")]
    public string? ColorHex { get; set; }

    [Range(0, double.MaxValue)] public decimal? Price { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    public bool? HasAccident { get; set; }

    public IFormFile[]? Photos { get; set; }
}