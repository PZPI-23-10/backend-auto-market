using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public record LoginUserRequest(
    [Required] string Email,
    [Required] string Password,
    bool RememberMe = false
);