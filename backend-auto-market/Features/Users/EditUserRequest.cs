namespace backend_auto_market.Features.Users;

public record EditUserRequest(
    string? Password = null,
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber = null,
    DateTime? DateOfBirth = null,
    IFormFile? Photo = null,
    string? Address = null,
    string? Country = null,
    string? AboutYourself = null);