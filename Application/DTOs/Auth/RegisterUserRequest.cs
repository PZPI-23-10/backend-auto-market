using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public record RegisterUserRequest(
    [EmailAddress] [Required] string Email,
    [Required] string FirstName,
    [Required] string LastName,
    [Required] string Country,
    [Required] DateTime DateOfBirth,
    string? Password = null,
    string? PhoneNumber = null,
    string? Address = null,
    string? AboutYourself = null
);