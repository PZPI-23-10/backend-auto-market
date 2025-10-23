namespace backend_auto_market.Features.Users;

public record LoginUserRequest(
    string Email,
    string Password,
    bool RememberMe);