using Application.DTOs.Auth;

namespace Application.DTOs.Profile;

public record UpdateProfileDto(
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber = null,
    DateTime? DateOfBirth = null,
    FileDto? Photo = null,
    string? Address = null,
    string? Country = null,
    string? AboutYourself = null
);