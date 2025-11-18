using Api.Extensions;
using Application.DTOs.Auth;
using Api.Models.Listings;
using Application.DTOs.Listings;
using Application.Interfaces.Services;

namespace Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

[ApiController]
[Route("api/[controller]")]
public class ListingController(IListingService listingService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleListingResponse>>> Get(
        [FromQuery] VehicleListingFilter? filter = null)
    {
        var listings = await listingService.GetPublishedListings(filter);
        return Ok(listings);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehicleListingResponse>> GetById(int id)
    {
        VehicleListingResponse listing = await listingService.GetListingById(id);

        return Ok(listing);
    }

    [HttpGet("user")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<IEnumerable<VehicleListingResponse>>> GetByAuthorizedUser()
    {
        var userId = User.GetUserId();
        IEnumerable<VehicleListingResponse> listings = await listingService.GetUserListings(userId);
        return Ok(listings);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateAndPublish([FromForm] PublishedVehicleListingRequest request)
    {
        int userId = User.GetUserId();

        int listingId = await listingService.CreateAndPublish(userId, ToPublishedCommand(request));

        return Ok(listingId);
    }

    [HttpPost("draft/{id:int}/publish")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> PublishDraft(int id, [FromForm] PublishedVehicleListingRequest request)
    {
        int userId = User.GetUserId();

        await listingService.PublishDraft(userId, id, ToPublishedCommand(request));

        return NoContent();
    }

    [HttpPost("draft")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateDraft([FromForm] DraftVehicleListingRequest request)
    {
        int userId = User.GetUserId();

        await listingService.CreateDraft(userId, ToDraftCommand(request));

        return NoContent();
    }

    [HttpPut("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdatePublished(int id, [FromForm] DraftVehicleListingRequest request)
    {
        int userId = User.GetUserId();

        await listingService.UpdatePublished(userId, id, ToDraftCommand(request));
        return NoContent();
    }

    [HttpPut("draft/{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateDraft(int id, [FromForm] DraftVehicleListingRequest request)
    {
        int userId = User.GetUserId();

        await listingService.UpdateDraft(userId, id, ToDraftCommand(request));
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Delete(int id)
    {
        int userId = User.GetUserId();

        await listingService.DeleteListing(id, userId);
        return NoContent();
    }

    public static DraftVehicleListingCommand ToDraftCommand(DraftVehicleListingRequest request)
    {
        List<FileDto> photos = (request.Photos ?? []).Select(x => new FileDto(x.FileName, x.OpenReadStream())).ToList();

        return new DraftVehicleListingCommand
        {
            ModelId = request.ModelId,
            BodyTypeId = request.BodyTypeId,
            ConditionId = request.ConditionId,
            CityId = request.CityId,
            Year = request.Year,
            Mileage = request.Mileage,
            Number = request.Number,
            ColorHex = request.ColorHex,
            Price = request.Price,
            Description = request.Description,
            HasAccident = request.HasAccident,
            Photos = photos
        };
    }

    public static PublishedVehicleListingCommand ToPublishedCommand(PublishedVehicleListingRequest request)
    {
        List<FileDto> photos = (request.Photos ?? []).Select(x => new FileDto(x.FileName, x.OpenReadStream())).ToList();

        return new PublishedVehicleListingCommand
        {
            ModelId = request.ModelId,
            BodyTypeId = request.BodyTypeId,
            ConditionId = request.ConditionId,
            CityId = request.CityId,
            Year = request.Year,
            Mileage = request.Mileage,
            Number = request.Number,
            ColorHex = request.ColorHex,
            Price = request.Price,
            Description = request.Description,
            HasAccident = request.HasAccident,
            Photos = photos
        };
    }
}