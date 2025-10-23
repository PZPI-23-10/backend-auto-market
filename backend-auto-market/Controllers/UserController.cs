using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using backend_auto_market.Features.Users;
using backend_auto_market.Persistence;
using backend_auto_market.Persistence.Models;
using backend_auto_market.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_auto_market.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(DataContext context, TokenService tokenService, IConfiguration configuration)
    : ControllerBase
{
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) ||
            string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName) ||
            string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.Country))
        {
            return BadRequest("All fields are required.");
        }

        if (context.Users.Any(u => u.Email == request.Email))
            return BadRequest("User with this email already exists.");

        if (request.Password.Length is < 5 or > 27)
            return BadRequest("Password must be between 5 and 20 characters.");

        if (!Regex.IsMatch(request.Password, @"^[a-zA-Z0-9!@#$%^&*]+$"))
            return BadRequest("Password can only contain letters, numbers, and special characters.");

        if (!Regex.IsMatch(request.Password, @"[!@#$%^&*]"))
            return BadRequest("Password must contain at least one special character.");

        var inputBytes = Encoding.UTF8.GetBytes(request.Password);
        var hashBytes = MD5.HashData(inputBytes);

        var user = new User
        {
            Email = request.Email,
            Password = Convert.ToBase64String(hashBytes),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Country = request.Country,
            DateOfBirth = request.DateOfBirth,
            Address = request.Address,
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            return BadRequest("All fields are required.");

        var user = context.Users.FirstOrDefault(u => u.Email == request.Email);
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
}