using System.ComponentModel.DataAnnotations;
using Application.DTOs.Listings;

namespace Api.Models.Listings;

public class DraftVehicleListingRequest
{
    public int? ModelId { get; set; }
    public int? BodyTypeId { get; set; }
    public int? GearTypeId { get; set; }
    public int? FuelTypeId { get; set; }
    public int? ConditionId { get; set; }
    public int? CityId { get; set; }
    [Range(1900, 2100)] public int? Year { get; set; }
    [Range(0, int.MaxValue)] public int? Mileage { get; set; }
    public string? Number { get; set; }
    public string? Vin { get; set; }

    [RegularExpression("^#([A-Fa-f0-9]{6})$")]
    [MaxLength(7)]
    public string? ColorHex { get; set; }

    [Range(0, double.MaxValue)] public decimal? Price { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    public bool? HasAccident { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int[]? PhotosToRemove { get; set; }
    public OrderedFileRequest[]? NewPhotos { get; set; }
    public ListingPhotoSortOrder[]? UpdatedPhotoSortOrder { get; set; }
}