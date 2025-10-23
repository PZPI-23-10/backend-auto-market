namespace backend_auto_market.Features.Users;

public record LoginUserRequest(
    string Username,
    string Password,
    bool RememberMe);