using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ListingController(IListingService listingService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var listings = await listingService.GetListingsAsync();
        return Ok(listings);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var listing = await listingService.GetListingByIdAsync(id);
        
        if (listing == null)
            return NotFound();

        return Ok(listing);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var listings = await listingService.GetUserListingsAsync(userId);
        return Ok(listings);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Create([FromBody] CreateVehicleListingRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var createdListing = await listingService.CreateListingAsync(request, userId.Value);
        
        return CreatedAtAction(nameof(GetById), new { id = createdListing.Id }, createdListing);
    }
    
    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVehicleListingRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        try
        {
            await listingService.UpdateListingAsync(id, request, userId.Value);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Listing not found.");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid("You are not authorized to update this listing.");
        }
    }
    
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        try
        {
            await listingService.DeleteListingAsync(id, userId.Value);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Listing not found.");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid("You are not authorized to delete this listing.");
        }
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out int id) ? id : null;
    }
}