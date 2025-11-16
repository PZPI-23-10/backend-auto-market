using Application.DTOs.Listings;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleListingController(IVehicleListingRepository vehicleListingRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vehicleListings = await vehicleListingRepository.GetAllAsync();
        return Ok(vehicleListings.Select(getVehicleListingResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vehicleListings = await vehicleListingRepository.GetByIdAsync(id);

        if (vehicleListings == null)
            return NotFound();

        return Ok(getVehicleListingResponse(vehicleListings));
    }

    private VehicleListingResponse getVehicleListingResponse(VehicleListing listing)
    {
        var response = new VehicleListingResponse()
        {
            Id = listing.Id,
            UserId = listing.UserId,
            ModelName = listing.Model?.Name,
            BrandName = listing.Model?.Brand?.Name,
            BodyTypeName = listing.BodyType?.Name,
            ConditionName = listing.Condition?.Name,
            CityName = listing.City?.Name,
            RegionName = listing.City?.Region?.Name,
            Year = listing.Year,
            Description = listing.Description,
            Mileage = listing.Mileage,
            HasAccident = listing.HasAccident ?? false,
            ColorHex = listing.ColorHex,
            Price = listing.Price ?? 0,
            Number = listing.Number ?? "",
            IsPublished = listing.IsPublished,
            PhotoUrls = listing.Photos.Select(p => p.PhotoUrl),
            CreatedAt = listing.Created.DateTime
        };
        
        return response;
    }
}