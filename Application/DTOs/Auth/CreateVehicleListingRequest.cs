using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public record CreateVehicleListingRequest(
    [Required] int ModelId,
    [Required] int BodyTypeId,
    [Required] int ConditionId,
    [Required] int CityId,
    [Required] int Year,
    [MaxLength(2000)] string Description,
    [Required] int Mileage,
    [Required] bool HasAccident,
    [MaxLength(7)] string ColorHex,
    [Required] decimal Price,
    // Принимаем список URL фотографий
    List<string>? PhotoUrls = null
    );