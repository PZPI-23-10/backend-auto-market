using Application.DTOs.Location;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CityController(ICityRepository cities, IRegionRepository regions) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityResponse>>> GetAll()
    {
        var allRegions = await cities.GetAllAsync();
        return Ok(allRegions.Select(MapEntityToDto));
    }

    [HttpGet("for-region/{regionId:int}")]
    public async Task<ActionResult<IEnumerable<CityResponse>>> GetByRegionId(int regionId)
    {
        var region = await regions.GetByIdAsync(regionId);

        if (region == null)
            return NotFound();

        return Ok(region.Cities.Select(MapEntityToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CityResponse>> GetById(int id)
    {
        var city = await cities.GetByIdAsync(id);

        if (city == null)
            return NotFound();

        return Ok(MapEntityToDto(city));
    }


    private CityResponse MapEntityToDto(City city)
    {
        var dto = new CityResponse
        {
            Id = city.Id,
            Name = city.Name
        };

        return dto;
    }
}