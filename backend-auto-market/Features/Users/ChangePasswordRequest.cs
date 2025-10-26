namespace backend_auto_market.Features.Users;

public record ChangePasswordRequest(
    string? Password = null,
    string NewPassword = null,
    string PasswordConfirmation = null );