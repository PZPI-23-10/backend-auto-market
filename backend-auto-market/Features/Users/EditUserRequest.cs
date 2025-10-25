namespace backend_auto_market.Features.Users;

public record EditUserRequest(
    string? Password = null,
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber = null,
    DateTime? DateOfBirth = null,
    string? UrlPhoto = null,
    string? Address = null,
    string? Country = null,
    string? AboutYourself = null);