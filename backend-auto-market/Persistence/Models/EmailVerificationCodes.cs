namespace backend_auto_market.Persistence.Models;

public class EmailVerificationCode
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime ExpirationTime { get; set; }
    public VerificationType Type { get; set; } 
}

public enum VerificationType
{
    Register = 1,
    PasswordReset = 2,
    PasswordChange = 3
}