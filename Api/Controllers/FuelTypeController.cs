using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuelTypeController(IFuelTypeRepository fuelTypes) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FuelTypeResponse>>> GetAll()
    {
        var vehicleModel = await fuelTypes.GetAllAsync();
        return Ok(vehicleModel.Select(GetResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FuelTypeResponse>> GetById(int id)
    {
        var vehicleModel = await fuelTypes.GetByIdAsync(id);

        if (vehicleModel == null)
            return NotFound();

        return Ok(GetResponse(vehicleModel));
    }

    private FuelTypeResponse GetResponse(FuelType entity)
    {
        return new FuelTypeResponse()
        {
            Id = entity.Id,
            Name = entity.Name,
        };
    }
}