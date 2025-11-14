using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public record ChangePasswordRequest(
    [Required] string Password,
    [Required] string NewPassword,
    [Required] string PasswordConfirmation
);