using Api.Extensions;
using Api.Models.Auth;
using Application.DTOs.Auth;
using Application.DTOs.Profile;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController(IProfileService profileService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserProfileResponse>> UserProfile([FromQuery] int userId)
    {
        var user = await profileService.GetUser(userId);
        return Ok(user);
    }

    [HttpGet]
    [Route("all")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<ActionResult<IEnumerable<UserProfileResponse>>> GetAllProfiles()
    {
        IEnumerable<UserProfileResponse> profiles = await profileService.GetAllUsers();
        return Ok(profiles);
    }

    [HttpPut]
    [Route("update")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Edit([FromForm] UpdateProfileRequest request)
    {
        int userId = User.GetUserId();

        UpdateProfileDto dto = new UpdateProfileDto
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Country = request.Country,
            AboutYourself = request.AboutYourself,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Address = request.Address,
            Photo = request.Photo != null ? new FileDto(request.Photo.FileName, request.Photo.OpenReadStream()) : null,
        };

        await profileService.UpdateProfile(userId, dto);

        return Ok();
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete([FromQuery] int userId)
    {
        await profileService.DeleteUser(userId);

        return Ok();
    }

    [HttpGet]
    [Route("roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetRoles()
    {
        return Ok(new[] { UserRoles.Admin, UserRoles.User });
    }

    [HttpPost]
    [Route("roles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> AssignRole([FromQuery] int userId, string role)
    {
        await profileService.AddToRole(userId, role);

        return Ok();
    }
}