namespace Application.DTOs.Listings;

public class VehicleListingFilter
{
    public int? VehicleTypeId { get; set; }
    public int? BrandId { get; set; }
    public int? ModelId { get; set; }
    public int? GearTypeId { get; set; }
    public int? BodyTypeId { get; set; }
    public int? ConditionId { get; set; }
    public int? FuelTypeId { get; set; }
    public int? CityId { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public int? MileageFrom { get; set; }
    public int? MileageTo { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public bool? HasAccident { get; set; }
}