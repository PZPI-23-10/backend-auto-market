using System.Text.RegularExpressions;
using backend_auto_market.Features.Users;
using backend_auto_market.Persistence;
using backend_auto_market.Persistence.Models;
using backend_auto_market.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_auto_market.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(DataContext dataContext, IConfiguration configuration, TokenService tokenService) : ControllerBase
{
    [HttpPost]
    [Route("RegisterUser")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) ||
            string.IsNullOrEmpty(request.Password) ||
            string.IsNullOrEmpty(request.FirstName) ||
            string.IsNullOrEmpty(request.LastName) ||
            string.IsNullOrEmpty(request.PhoneNumber) ||
            string.IsNullOrEmpty(request.Country))
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
            AboutYourself = request.AboutYourself
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
}