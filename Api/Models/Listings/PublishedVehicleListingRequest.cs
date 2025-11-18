using System.ComponentModel.DataAnnotations;

namespace Api.Models.Listings;

public class PublishedVehicleListingRequest
{
    [Required] public int ModelId { get; set; }
    [Required] public int BodyTypeId { get; set; }
    [Required] public int GearTypeId { get; set; }
    [Required] public int FuelTypeId { get; set; }
    [Required] public int ConditionId { get; set; }
    [Required] public int CityId { get; set; }
    [Required] public int Year { get; set; }
    [Required] public int Mileage { get; set; }
    [Required] public string Number { get; set; }
    [Required] public string ColorHex { get; set; }
    [Required] public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool HasAccident { get; set; } = false;
    public IFormFile[]? Photos { get; set; }
}