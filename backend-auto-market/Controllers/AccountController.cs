using System.Security.Claims;
using System.Text.RegularExpressions;
using backend_auto_market.Features.Users;
using backend_auto_market.Persistence;
using backend_auto_market.Persistence.Models;
using backend_auto_market.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_auto_market.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(DataContext dataContext, IConfiguration configuration, TokenService tokenService)
    : ControllerBase
{
    [HttpPost]
    [Route("RegisterUser")]
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

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Country = request.Country,
            Email = request.Email,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            AboutYourself = request.AboutYourself,
            Address = request.Address
        };

        await dataContext.Users.AddAsync(user);
        await dataContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("LoginUser")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            return BadRequest("All fields are required.");

        var user = dataContext.Users.FirstOrDefault(u => u.Email == request.Email);
        if (user == null)
            return Unauthorized("User with this email does not exist.");

        if (user.IsGoogleAuth)
            return BadRequest("This user is registered with Google authentication. Please use Google login.");

        if (user.Password != request.Password)
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
    [Route("Edit")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Edit([FromBody] EditUserRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest("User ID not found.");

        if (!int.TryParse(userIdClaim, out var userId))
            return BadRequest("User ID is not an integer.");

        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return NotFound();

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Country = request.Country;
        user.AboutYourself = request.AboutYourself;
        user.Address = request.Address;
        user.DateOfBirth = request.DateOfBirth;
        user.UrlPhoto = request.UrlPhoto;
        user.PhoneNumber = request.PhoneNumber;

        await dataContext.SaveChangesAsync();
        return Ok();
    }
}