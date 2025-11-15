using Application.DTOs.Listings;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListingController(IListingService listingService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        IEnumerable<VehicleListingDto> listings = await listingService.GetListings();

        return Ok(listings);
    }

    [HttpGet]
    public async Task<IActionResult> Get(int userId)
    {
        IEnumerable<VehicleListingDto> listings = await listingService.GetUserListings(userId);

        return Ok(listings);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Create([FromBody] CreateVehicleListingRequest request)
    {
        await listingService.CreateListing(request);

        return Ok();
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Update([FromBody] UpdateListingRequest request)
    {
        await listingService.UpdateListing(request);

        return Ok();
    }
}