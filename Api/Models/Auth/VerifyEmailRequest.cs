using System.ComponentModel.DataAnnotations;

namespace Api.Models.Auth;

public class VerifyEmailRequest
{
    [Required] public string Code { get; set; }
}