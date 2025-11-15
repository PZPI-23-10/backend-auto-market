using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public record CreateUserRequest(
    [Required] string Email,
    [Required] string Password,
    [Required] string FirstName,
    [Required] string LastName,
    [Required] string PhoneNumber,
    [Required] string Country,
    [Required] DateTime DateOfBirth,
    string? Address = null
);