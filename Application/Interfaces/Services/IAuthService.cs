using Application.DTOs.Auth;
using Application.Enums;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginUserResponse> RegisterUser(RegisterUserRequest request);
    Task<LoginUserResponse> LoginUser(LoginUserRequest request);
    Task<LoginUserResponse> LoginWithGoogle(GoogleLoginRequest request, GoogleAuthPlatform platform);
    Task<bool> IsEmailExists(string email);
    Task ChangePassword(int userId, ChangePasswordRequest request);
    Task<ResetPasswordResult> RequestPasswordReset(ForgotPasswordRequest request);
    Task ConfirmPasswordReset(string userEmail, string encodedToken, string newPassword);
}