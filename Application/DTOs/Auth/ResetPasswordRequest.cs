using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public record ResetPasswordRequest(
    [Required] string Email,
    [Required] string Token,
    [Required] string NewPassword
);