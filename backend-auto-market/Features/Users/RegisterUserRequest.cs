namespace backend_auto_market.Features.Users;

public record RegisterUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string Country,
    DateTime DateOfBirth,
    string? Password = null,
    string? PhoneNumber = null,
    string? Address = null,
    string? AboutYourself = null
    );