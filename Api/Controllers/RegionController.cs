using Application.DTOs.Location;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegionController(IRegionRepository regions) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegionResponse>>> GetAll()
    {
        var allRegions = await regions.GetAllAsync();
        return Ok(allRegions.Select(MapEntityToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RegionResponse>> GetById(int id)
    {
        var region = await regions.GetByIdAsync(id);

        if (region == null)
            return NotFound();

        return Ok(MapEntityToDto(region));
    }

    private RegionResponse MapEntityToDto(Region region)
    {
        var dto = new RegionResponse
        {
            Id = region.Id,
            Name = region.Name
        };

        return dto;
    }
}