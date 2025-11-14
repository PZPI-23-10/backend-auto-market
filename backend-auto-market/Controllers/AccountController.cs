using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using backend_auto_market.Features.Users;
using backend_auto_market.Persistence;
using backend_auto_market.Persistence.Models;
using backend_auto_market.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    IFileStorage fileStorage,
    EmailService emailService,
    IMemoryCache memoryCache,
    IPasswordHasher passwordHasher
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

        string randomVerificationCode = new Random().Next(100000, 999999).ToString();

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
    [Route("send-verification-email")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SendVerificationEmail()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("User ID not found or invalid.");

        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            return NotFound("User not found.");

        if (user.IsVerified)
            return BadRequest("Email already verified.");

        var existingCode = await dataContext.EmailVerificationCodes
            .FirstOrDefaultAsync(x => x.UserId == userId);
        if (existingCode != null)
        {
            dataContext.EmailVerificationCodes.Remove(existingCode);
        }

        string newVerificationCode = new Random().Next(100000, 999999).ToString();

        EmailVerificationCode verificationCodeRecord = new EmailVerificationCode
        {
            Code = newVerificationCode,
            ExpirationTime = DateTime.UtcNow.AddMinutes(15),
            Type = VerificationType.Register,
            UserId = userId
        };

        await dataContext.EmailVerificationCodes.AddAsync(verificationCodeRecord);
        await dataContext.SaveChangesAsync();

        try
        {
            await emailService.SendRegistrationEmail(user.Email, user.FirstName, newVerificationCode);
            return Ok("Verification code sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending verification email: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to send verification email.");
        }
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

        if (!passwordHasher.Verify(request.Password, user.Password))
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
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("User ID not found or invalid.");

        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            return NotFound();

        if (string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.Password) ||
            string.IsNullOrEmpty(request.PasswordConfirmation))
        {
            return BadRequest("All fields are required.");
        }

        if (!passwordHasher.Verify(request.Password, user.Password))
            return BadRequest("Old password is incorrect.");

        if (request.NewPassword.Length < 5 || request.NewPassword.Length > 27)
            return BadRequest("Password must be between 5 and 27 characters.");

        if (!Regex.IsMatch(request.NewPassword, @"^[a-zA-Z0-9!@#$%^&*()_+=\-{}\[\]:;""'<>,.?/~`|\\]+$"))
            return BadRequest("Password can only contain letters, numbers, and special characters.");

        if (request.NewPassword != request.PasswordConfirmation)
            return BadRequest("New password and confirmation do not match.");

        user.Password = passwordHasher.Hash(request.NewPassword);

        dataContext.Users.Update(user);
        await dataContext.SaveChangesAsync();

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
            user.UrlPhoto = await fileStorage.UploadAvatar(stream, request.Photo.FileName, userId);
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

        var tempKey = Guid.NewGuid().ToString();
        memoryCache.Set(tempKey, passwordHasher.Hash(request.Password), TimeSpan.FromMinutes(15));

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