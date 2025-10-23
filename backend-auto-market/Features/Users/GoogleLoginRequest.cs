namespace backend_auto_market.Features.Users;

public record GoogleLoginRequest(
    string GoogleToken,
    bool RememberMe);