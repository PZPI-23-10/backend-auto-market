using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

public class EmailVerificationCode : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public DateTime ExpirationTime { get; set; }
    public VerificationType Type { get; set; }
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))] public User User { get; set; }
}