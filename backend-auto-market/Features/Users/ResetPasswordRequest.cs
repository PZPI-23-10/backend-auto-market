namespace backend_auto_market.Features.Users;

public record ResetPasswordRequest(
    string Password,
    string PasswordConfirmation,
    string Email);