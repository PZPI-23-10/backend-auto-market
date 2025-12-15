namespace Application.DTOs.Listings;

public class DraftVehicleListingCommand
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
    public string? Vin { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public IEnumerable<OrderedFileDto>? NewPhotos { get; set; }
    public IEnumerable<int>? PhotosToRemove { get; set; }

    public IEnumerable<ListingPhotoSortOrder>? UpdatedPhotoSortOrder { get; set; }
}