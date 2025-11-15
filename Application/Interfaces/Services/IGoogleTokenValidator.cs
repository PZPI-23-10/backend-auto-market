using Application.DTOs.Auth;
using Application.Enums;

namespace Application.Interfaces.Services;

public interface IGoogleTokenValidator
{
    Task<GoogleAuthResult> ValidateAsync(string token, GoogleAuthPlatform platform);
}