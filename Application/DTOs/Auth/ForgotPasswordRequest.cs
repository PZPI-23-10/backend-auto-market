using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public record ForgotPasswordRequest([Required] string Email, [Required] string ClientUrl);