using System.ComponentModel.DataAnnotations.Schema;

namespace backend_auto_market.Persistence.Models;

public class EmailVerificationCode : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public DateTime ExpirationTime { get; set; }
    public VerificationType Type { get; set; }
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))] public User User { get; set; }
}

public enum VerificationType
{
    Register = 1,
    PasswordReset = 2,
    PasswordChange = 3
}