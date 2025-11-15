using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public record GoogleLoginRequest(
    [Required] string GoogleToken,
    bool RememberMe = false
);