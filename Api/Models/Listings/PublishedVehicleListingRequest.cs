using System.ComponentModel.DataAnnotations;
using Application.DTOs.Listings;

namespace Api.Models.Listings;

public class PublishedVehicleListingRequest
{
    [Required] public int ModelId { get; set; }
    [Required] public int BodyTypeId { get; set; }
    [Required] public int GearTypeId { get; set; }
    [Required] public int FuelTypeId { get; set; }
    [Required] public int ConditionId { get; set; }
    [Required] public int CityId { get; set; }
    [Required] [Range(1900, 2100)] public int Year { get; set; }
    [Required] [Range(0, int.MaxValue)] public int Mileage { get; set; }
    [Required] public string Number { get; set; }

    [Required]
    [RegularExpression("^#([A-Fa-f0-9]{6})$")]
    [MaxLength(7)]
    public string ColorHex { get; set; }

    [Required] [Range(0, double.MaxValue)] public decimal Price { get; set; }
    [MaxLength(2000)] public string? Description { get; set; } = string.Empty;
    public bool HasAccident { get; set; } = false;

    public int[]? PhotosToRemove { get; set; }
    public OrderedFileRequest[]? NewPhotos { get; set; }
    public ListingPhotoSortOrder[]? UpdatedPhotoSortOrder { get; set; }
}