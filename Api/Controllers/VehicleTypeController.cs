using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleTypeController(IVehicleTypeRepository vehicleTypes) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleTypeResponse>>> GetAll()
    {
        var vehicleModel = await vehicleTypes.GetAllAsync();
        return Ok(vehicleModel.Select(GetResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehicleTypeResponse>> GetById(int id)
    {
        var vehicleModel = await vehicleTypes.GetByIdAsync(id);

        if (vehicleModel == null)
            return NotFound();

        return Ok(GetResponse(vehicleModel));
    }

    private VehicleTypeResponse GetResponse(VehicleType entity)
    {
        return new VehicleTypeResponse
        {
            Id = entity.Id,
            Name = entity.Name,
        };
    }
}