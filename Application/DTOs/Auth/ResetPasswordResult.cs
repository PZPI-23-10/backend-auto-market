namespace Application.DTOs.Auth;

public record ResetPasswordResult(int UserId, string Token);