namespace Application.DTOs.Auth;

public record VehicleListingResponse(
    int Id,
    int UserId,
    string BrandName,      
    string ModelName,
    string BodyTypeName,
    string ConditionName,
    string CityName,
    int Year,
    string Description,
    int Mileage,
    bool HasAccident,
    string ColorHex,
    decimal Price,
    List<string> PhotoUrls,  
    DateTime CreatedAt       
                             );