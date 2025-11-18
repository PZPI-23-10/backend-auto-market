using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleBodyTypeController(
    IVehicleBodyTypeRepository vehicleBodyType) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleBodyTypeResponse>>> GetAll()
    {
        var bodyTypes = await vehicleBodyType.GetAllAsync();
        return Ok(bodyTypes.Select(GetResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehicleBodyTypeResponse>> GetById(int id)
    {
        var bodyType = await vehicleBodyType.GetByIdAsync(id);

        if (bodyType == null)
            return NotFound();

        return Ok(GetResponse(bodyType));
    }

    private VehicleBodyTypeResponse GetResponse(VehicleBodyType entity)
    {
        var response = new VehicleBodyTypeResponse
        {
            Id = entity.Id,
            Name = entity.Name,
        };

        return response;
    }
}