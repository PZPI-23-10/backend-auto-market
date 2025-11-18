using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GearTypeController(IGearTypeRepository gearTypes) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vehicleModel = await gearTypes.GetAllAsync();
        return Ok(vehicleModel.Select(GetResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vehicleModel = await gearTypes.GetByIdAsync(id);

        if (vehicleModel == null)
            return NotFound();

        return Ok(GetResponse(vehicleModel));
    }

    private GearTypeResponse GetResponse(GearType entity)
    {
        return new GearTypeResponse()
        {
            Id = entity.Id,
            Name = entity.Name,
        };
    }
}