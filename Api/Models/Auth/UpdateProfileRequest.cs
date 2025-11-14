namespace Api.Models.Auth;

public record UpdateProfileRequest(
    string? Password = null,
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber = null,
    DateTime? DateOfBirth = null,
    IFormFile? Photo = null,
    string? Address = null,
    string? Country = null,
    string? AboutYourself = null
);