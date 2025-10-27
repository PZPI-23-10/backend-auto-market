namespace backend_auto_market.Features.Users;

public record ChangePasswordRequest(
    string Password,
    string NewPassword,
    string PasswordConfirmation
);