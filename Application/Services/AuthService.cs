using System.ComponentModel.DataAnnotations;
using Application.DTOs.Auth;
using Application.Enums;
using Application.Exceptions;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

public class AuthService(
    IUserRepository users,
    IVerificationService verificationService,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    IGoogleTokenValidator googleTokenValidator,
    IMemoryCache memoryCache,
    IUrlSafeEncoder urlSafeEncoder,
    IEmailSender emailSender
) : IAuthService
{
    public async Task<LoginUserResponse> RegisterUser(RegisterUserRequest request)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Country = request.Country,
            Email = request.Email,
            Password = passwordHasher.Hash(request.Password),
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth.ToUniversalTime(),
            AboutYourself = request.AboutYourself,
            Address = request.Address,
            IsVerified = false,
        };

        await verificationService.SendRegisterCode(user);

        await unitOfWork.SaveChangesAsync();

        Token accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Email, false);

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

        if (!passwordHasher.Verify(request.Password, user.Password))
            throw new UnauthorizedException("Invalid Password.");

        Token accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Email, request.RememberMe);

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
                UrlPhoto = validationResult.PhotoUrl,
                Email = validationResult.Email,
                IsGoogleAuth = true
            };

            await users.AddAsync(user);
            await unitOfWork.SaveChangesAsync();
        }

        Token accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Email, request.RememberMe);
        var loginUserResponse = new LoginUserResponse(user.Id.ToString(), accessToken.TokenKey);

        return loginUserResponse;
    }

    public async Task<bool> IsEmailExists(string email)
    {
        return await users.UserWithEmailExists(email);
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

        if (!passwordHasher.Verify(request.Password, user.Password))
            throw new UnauthorizedException("Invalid Password.");

        user.Password = passwordHasher.Hash(request.NewPassword);

        users.Update(user);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<ResetPasswordResult> RequestPasswordReset(ResetPasswordRequest request)
    {
        var user = await users.GetUserByEmail(request.Email);

        if (user == null)
            throw new NotFoundException("User with this email does not exist.");

        if (user.IsGoogleAuth)
        {
            throw new ValidationException(
                "This user is registered with Google authentication. Please use Google login.");
        }

        var tempKey = Guid.NewGuid().ToString();
        memoryCache.Set(tempKey, passwordHasher.Hash(request.Password), TimeSpan.FromMinutes(15));

        string token = urlSafeEncoder.EncodeString(tempKey);

        return new ResetPasswordResult(user.Id, token);
    }

    public async Task ConfirmPasswordReset(int userId, string token)
    {
        var user = await users.GetByIdAsync(userId);

        if (user == null)
            throw new NotFoundException("User not found.");

        if (user.IsGoogleAuth)
        {
            throw new ValidationException(
                "This user is registered with Google authentication. Please use Google login.");
        }

        string tempKey = urlSafeEncoder.DecodeString(token);

        if (!memoryCache.TryGetValue(tempKey, out string? newPassword))
            throw new ValidationException("Link expired or invalid.");

        user.Password = newPassword;

        users.Update(user);
        await unitOfWork.SaveChangesAsync();

        memoryCache.Remove(tempKey);
        await emailSender.SendEmailAsync(user.Email, "Пароль змінено", "Пароль успішно змінено.");
    }
}