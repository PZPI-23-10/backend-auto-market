using System.Security.Claims;
using Application.DTOs;
using Application.DTOs.Favourite;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavouriteController : ControllerBase
{
    private readonly IFavouriteService _favouriteService;

    public FavouriteController(IFavouriteService favouriteService)
    {
        _favouriteService = favouriteService;
    }

    [HttpPost("add")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AddToFavourites([FromBody] FavouriteDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _favouriteService.AddToFavouritesAsync(userId, dto.VehicleListingId);

        if (!result)
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpDelete("remove")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> RemoveFromFavourites([FromBody] FavouriteDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _favouriteService.RemoveFromFavouritesAsync(userId, dto.VehicleListingId);

        if (!result)
        {
            return NotFound();
        }

        return Ok();
    }
    
    private int GetCurrentUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("Не удалось определить ID пользователя из токена.");
    }
}