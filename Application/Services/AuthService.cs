using System.ComponentModel.DataAnnotations;
using Application.DTOs.Auth;
using Application.Enums;
using Application.Exceptions;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class AuthService(
    IUserRepository users,
    IVerificationService verificationService,
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    IGoogleTokenValidator googleTokenValidator,
    IUrlSafeEncoder urlSafeEncoder,
    IEmailSender emailSender,
    UserManager<User> userManager) : IAuthService
{
    public async Task<LoginUserResponse> RegisterUser(RegisterUserRequest request)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Email,
            Country = request.Country,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth.ToUniversalTime(),
            AboutYourself = request.AboutYourself,
            Address = request.Address,
            EmailConfirmed = false,
        };

        IdentityResult createResult = await userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
            throw new InvalidOperationException(
                $"Register failed with errors: {string.Join(',', createResult.Errors.Select(e => e.Code))}");

        await verificationService.SendRegisterCode(user);
        await unitOfWork.SaveChangesAsync();

        await userManager.AddToRoleAsync(user, UserRoles.User);

        Token accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Email, [UserRoles.User], false);

        return new LoginUserResponse(user.Id.ToString(), accessToken.TokenKey);
    }

    public async Task<LoginUserResponse> LoginUser(LoginUserRequest request)
    {
        User? user = await users.GetUserByEmail(request.Email);

        if (user == null)
            throw new UnauthorizedException("User with this email does not exist.");

        if (user.IsGoogleAuth)
            throw new ValidationException(
                "This user is registered with Google authentication. Please use Google login.");

        if (!await userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException("Invalid Password.");

        IList<string> roles = await userManager.GetRolesAsync(user);
        Token accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Email, roles, request.RememberMe);

        return new LoginUserResponse(user.Id.ToString(), accessToken.TokenKey);
    }

    public async Task<LoginUserResponse> LoginWithGoogle(GoogleLoginRequest request, GoogleAuthPlatform platform)
    {
        GoogleAuthResult validationResult = await googleTokenValidator.ValidateAsync(request.GoogleToken, platform);

        var user = await users.GetUserByEmail(validationResult.Email);

        if (user == null)
        {
            user = new User
            {
                FirstName = validationResult.FirstName,
                LastName = validationResult.LastName,
                Avatar = new UserAvatar
                {
                    Url = validationResult.PhotoUrl,
                    IsExternal = true,
                },
                Email = validationResult.Email,
                IsGoogleAuth = true,
                EmailConfirmed = true,
                UserName = validationResult.Email,
            };

            IdentityResult result = await userManager.CreateAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Login failed with errors: {string.Join(',', result.Errors.Select(e => e.Code))}");

            await userManager.AddToRoleAsync(user, UserRoles.User);
        }

        IList<string> roles = await userManager.GetRolesAsync(user);

        Token accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Email, roles, request.RememberMe);
        var loginUserResponse = new LoginUserResponse(user.Id.ToString(), accessToken.TokenKey);

        return loginUserResponse;
    }

    public async Task<bool> IsEmailExists(string email)
    {
        return await userManager.FindByEmailAsync(email) != null;
    }

    public async Task ChangePassword(int userId, ChangePasswordRequest request)
    {
        var user = await users.GetByIdAsync(userId);

        if (user == null)
            throw new NotFoundException("User not found.");

        if (user.IsGoogleAuth)
        {
            throw new ValidationException(
                "This user is registered with Google authentication. Please use Google login.");
        }

        IdentityResult result = await userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);

        if (!result.Succeeded)
            throw new UnauthorizedException("Invalid Password.");
    }

    public async Task<ResetPasswordResult> RequestPasswordReset(ForgotPasswordRequest request)
    {
        var user = await users.GetUserByEmail(request.Email);

        if (user == null)
            throw new NotFoundException("User with this email does not exist.");

        if (user.IsGoogleAuth)
        {
            throw new ValidationException(
                "This user is registered with Google authentication. Please use Google login.");
        }

        string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        string urlSafeToken = urlSafeEncoder.EncodeString(resetToken);

        return new ResetPasswordResult(user.Id, urlSafeToken);
    }

    public async Task ConfirmPasswordReset(string userEmail, string encodedToken, string newPassword)
    {
        var user = await userManager.FindByEmailAsync(userEmail);

        if (user == null)
            throw new NotFoundException("User not found.");

        if (user.IsGoogleAuth)
        {
            throw new ValidationException(
                "This user is registered with Google authentication. Please use Google login.");
        }

        string token = urlSafeEncoder.DecodeString(encodedToken);

        await userManager.ResetPasswordAsync(user, token, newPassword);
        await emailSender.SendEmailAsync(user.Email, "Пароль змінено", "Пароль успішно змінено.");
    }
}