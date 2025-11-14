using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public record ResetPasswordRequest(
    [Required] string Password,
    [Required] string PasswordConfirmation,
    [Required] string Email
);