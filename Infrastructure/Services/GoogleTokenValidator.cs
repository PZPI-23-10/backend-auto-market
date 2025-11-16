using Application.DTOs.Auth;
using Application.Enums;
using Application.Interfaces.Services;
using Google.Apis.Auth;

namespace Infrastructure.Services;

public class GoogleTokenValidator(GoogleAuthSettings googleAuthSettings) : IGoogleTokenValidator
{
    public async Task<GoogleAuthResult> ValidateAsync(string token, GoogleAuthPlatform platform)
    {
        string clientId = platform switch
        {
            GoogleAuthPlatform.Android => googleAuthSettings.AndroidClientId,
            GoogleAuthPlatform.Web => googleAuthSettings.WebClientId,
            _ => throw new NotImplementedException()
        };
        
        var validationSettings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [clientId]
        };

        GoogleJsonWebSignature.Payload? payload = await GoogleJsonWebSignature.ValidateAsync(token, validationSettings);

        GoogleAuthResult result = new GoogleAuthResult
        {
            Email = payload.Email,
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
            PhotoUrl = payload.Picture
        };

        return result;
    }
}