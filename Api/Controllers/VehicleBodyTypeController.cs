using Application.Interfaces.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleBodyTypeController(
    IVehicleBodyTypeRepository vehicleBodyType) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vehicleType = await vehicleBodyType.GetAllAsync();
        return Ok(vehicleType);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vehicleType = await vehicleBodyType.GetByIdAsync(id);

        if (vehicleType == null)
            return NotFound();

        return Ok(vehicleType);
    }
}