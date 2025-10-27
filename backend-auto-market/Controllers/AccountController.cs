using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using backend_auto_market.Features.Users;
using backend_auto_market.Persistence;
using backend_auto_market.Persistence.Models;
using backend_auto_market.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ResetPasswordRequest = backend_auto_market.Features.Users.ResetPasswordRequest;

namespace backend_auto_market.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(
    DataContext dataContext,
    IConfiguration configuration,
    TokenService tokenService,
    Cloudinary cloudinary,
    EmailService emailService,
    IMemoryCache memoryCache
)
    : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) ||
            string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName) ||
            string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.Country))
        {
            return BadRequest("All fields are required.");
        }

        if (dataContext.Users.Any(u => u.Email == request.Email))
            return BadRequest("User with this email already exists.");

        if (request.Password.Length is < 5 or > 27)
            return BadRequest("Password must be between 5 and 20 characters.");

        if (!Regex.IsMatch(request.Password, @"^[a-zA-Z0-9!@#$%^&*]+$"))
            return BadRequest("Password can only contain letters, numbers, and special characters.");

        byte[] inputBytes = Encoding.UTF8.GetBytes(request.Password);
        byte[] hashBytes = MD5.HashData(inputBytes);

        string randomVerificationCode = new Random().Next(100000, 999999).ToString();

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Country = request.Country,
            Email = request.Email,
            Password = Convert.ToBase64String(hashBytes),
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth.ToUniversalTime(),
            AboutYourself = request.AboutYourself,
            Address = request.Address,
            IsVerified = false,
        };

        EmailVerificationCode verificationCode = new EmailVerificationCode
        {
            Code = randomVerificationCode,
            ExpirationTime = DateTime.UtcNow.AddMinutes(15),
            Type = VerificationType.Register,
            User = user
        };

        await dataContext.EmailVerificationCodes.AddAsync(verificationCode);
        await dataContext.SaveChangesAsync();

        await emailService.SendRegistrationEmail(user.Email, user.FirstName, randomVerificationCode);

        var accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Email, false);
        return Ok(new LoginUserResponse(user.Id.ToString(), accessToken.TokenKey));
    }

    [HttpPost]
    [Route("verify-email")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest("User ID not found.");

        if (!int.TryParse(userIdClaim, out var userId))
            return BadRequest("User ID is not an integer.");

        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return NotFound();

        if (user.IsVerified)
            return BadRequest("Email already verified.");

        EmailVerificationCode? verificationCode =
            dataContext.EmailVerificationCodes.FirstOrDefault(x => x.UserId == userId);

        if (verificationCode == null)
            return BadRequest("Email verification code not found.");

        if (verificationCode.Code != request.Code)
            return BadRequest("Invalid verification code.");

        if (verificationCode.ExpirationTime < DateTime.UtcNow)
            return BadRequest("Verification code has expired.");

        user.IsVerified = true;

        dataContext.Users.Update(user);
        dataContext.EmailVerificationCodes.Remove(verificationCode);

        await dataContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            return BadRequest("All fields are required.");

        var user = dataContext.Users.FirstOrDefault(u => u.Email == request.Email);
        if (user == null)
            return Unauthorized("User with this email does not exist.");

        if (user.IsGoogleAuth)
            return BadRequest("This user is registered with Google authentication. Please use Google login.");

        byte[] inputBytes = Encoding.UTF8.GetBytes(request.Password);
        byte[] hashBytes = MD5.HashData(inputBytes);
        string hashedPassword = Convert.ToBase64String(hashBytes);

        if (user.Password != hashedPassword)
            return Unauthorized("Invalid Password.");

        var accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Email, request.RememberMe);
        return Ok(new LoginUserResponse(user.Id.ToString(), accessToken.TokenKey));
    }

    [HttpPost("android/google")]
    public async Task<IActionResult> GoogleAndroidLogin([FromBody] GoogleLoginRequest request)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(request.GoogleToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [configuration.GetSection("Google")["AndroidClientId"]]
            });

        return await GoogleLogin(request, payload);
    }

    [HttpPost("web/google")]
    public async Task<IActionResult> GoogleWebLogin([FromBody] GoogleLoginRequest request)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(request.GoogleToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [configuration.GetSection("Google")["WebClientId"]]
            });

        return await GoogleLogin(request, payload);
    }

    [HttpGet("emailExists")]
    public async Task<IActionResult> IsEmailExists([FromQuery] string email) =>
        Ok(await dataContext.Users.AnyAsync(u => u.Email == email));

    private async Task<IActionResult> GoogleLogin(GoogleLoginRequest request, GoogleJsonWebSignature.Payload payload)
    {
        var user = await dataContext.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

        if (user == null)
        {
            user = new User
            {
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                UrlPhoto = payload.Picture,
                Email = payload.Email,
                IsGoogleAuth = true
            };

            dataContext.Users.Add(user);
            await dataContext.SaveChangesAsync();
        }

        var accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Email, request.RememberMe);
        return Ok(new LoginUserResponse(user.Id.ToString(), accessToken.TokenKey));
    }


    [HttpGet]
    public async Task<IActionResult> UserProfile([FromQuery] int userId)
    {
        if (userId < 0)
            return BadRequest("UserId must be greater than zero.");

        var user = await dataContext.Users.FindAsync(userId);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    [Route("ChangePassword")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest("User ID not found.");

        if (!int.TryParse(userIdClaim, out var userId))
            return BadRequest("User ID is not an integer.");

        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return NotFound();

        if (string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.Password) ||
            string.IsNullOrEmpty(request.PasswordConfirmation))
        {
            return BadRequest("All fields are required.");
        }

        byte[] inputBytes = Encoding.UTF8.GetBytes(request.Password);
        byte[] hashBytes = MD5.HashData(inputBytes);
        string oldHashedPassword = Convert.ToBase64String(hashBytes);

        if (user.Password != oldHashedPassword)
            return BadRequest("Old password is incorrect.");

        if (request.NewPassword.Length is < 5 or > 27)
            return BadRequest("Password must be between 5 and 20 characters.");

        if (!Regex.IsMatch(request.NewPassword, @"^[a-zA-Z0-9!@#$%^&*]+$"))
            return BadRequest("Password can only contain letters, numbers, and special characters.");

        if (request.NewPassword != request.PasswordConfirmation)
            return BadRequest("New password and confirmation do not match.");


        byte[] newPassBytes = Encoding.UTF8.GetBytes(request.NewPassword);
        byte[] newHashBytes = MD5.HashData(newPassBytes);
        string newHashedPassword = Convert.ToBase64String(newHashBytes);

        var tempKey = Guid.NewGuid().ToString();
        memoryCache.Set(tempKey, newHashedPassword, TimeSpan.FromMinutes(15));

        var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tempKey));

        var callbackUrl = Url.Action(
            nameof(ConfirmPasswordChange), "Account",
            new { userId = user.Id, token = token },
            protocol: Request.Scheme);

        await emailService.SendEmailAsync(
            user.Email,
            "Підтвердження зміни пароля",
            $"Для підтвердження зміни пароля перейдіть по посиланню: {callbackUrl}");

        return Ok();
    }

    [HttpGet("confirm-password-change")]
    public async Task<IActionResult> ConfirmPasswordChange([FromQuery] int userId, [FromQuery] string token)
    {
        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return NotFound();

        var tempKey = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        if (!memoryCache.TryGetValue(tempKey, out string? newPassword))
            return BadRequest("Час дії посилання сплинув або посилання недійсне.");

        user.Password = newPassword;

        dataContext.Users.Update(user);
        await dataContext.SaveChangesAsync();

        memoryCache.Remove(tempKey);
        await emailService.SendEmailAsync(user.Email, "Пароль змінено", "Пароль успішно змінено.");

        return Ok();
    }

    [HttpPut]
    [Route("edit")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Edit([FromForm] EditUserRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest("User ID not found.");

        if (!int.TryParse(userIdClaim, out var userId))
            return BadRequest("User ID is not an integer.");

        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return NotFound();

        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;
        user.Country = request.Country ?? user.Country;
        user.AboutYourself = request.AboutYourself ?? user.AboutYourself;
        user.Address = request.Address ?? user.Address;
        user.DateOfBirth = request.DateOfBirth ?? user.DateOfBirth;
        user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;

        if (request.Photo is { Length: > 0 })
        {
            await using var stream = request.Photo.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(request.Photo.FileName, stream),

                PublicId = $"avatars/{userId}/{Guid.NewGuid()}",

                Transformation = new Transformation()
                    .Width(500).Height(500).Crop("fill").Gravity("face")
            };

            ImageUploadResult? uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error == null)
            {
                user.UrlPhoto = uploadResult.SecureUrl.ToString();
            }
        }

        await dataContext.SaveChangesAsync();
        return Ok();
    }


    [HttpPost]
    [Route("PasswordReset")]
    public async Task<IActionResult> PasswordReset([FromBody] ResetPasswordRequest request)
    {
        var user = await dataContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return NotFound();

        if (string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.PasswordConfirmation))
        {
            return BadRequest("All fields are required.");
        }

        if (request.Password.Length is < 5 or > 27)
            return BadRequest("Password must be between 5 and 20 characters.");

        if (!Regex.IsMatch(request.Password, @"^[a-zA-Z0-9!@#$%^&*]+$"))
            return BadRequest("Password can only contain letters, numbers, and special characters.");

        if (request.Password != request.PasswordConfirmation)
            return BadRequest("New password and confirmation do not match.");


        byte[] newPassBytes = Encoding.UTF8.GetBytes(request.Password);
        byte[] newHashBytes = MD5.HashData(newPassBytes);
        string newHashedPassword = Convert.ToBase64String(newHashBytes);

        var tempKey = Guid.NewGuid().ToString();
        memoryCache.Set(tempKey, newHashedPassword, TimeSpan.FromMinutes(15));

        var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tempKey));

        var callbackUrl = Url.Action(
            nameof(ConfirmPasswordChange), "Account",
            new { userId = user.Id, token = token },
            protocol: Request.Scheme);

        await emailService.SendEmailAsync(
            user.Email,
            "Підтвердження зміни пароля",
            $"Для підтвердження зміни пароля перейдіть по посиланню: {callbackUrl}");

        return Ok();
    }
}