using Api.Extensions;
using Api.Models.Auth;
using Application.DTOs.Auth;
using Application.DTOs.Profile;
using Application.Interfaces.Services;
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
}