using Application.Interfaces.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleConditionController(IVehicleConditionRepository vehicleConditionRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vehicleCondition = await vehicleConditionRepository.GetAllAsync();
        return Ok(vehicleCondition);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vehicleCondition = await vehicleConditionRepository.GetByIdAsync(id);

        if (vehicleCondition == null)
            return NotFound();

        return Ok(vehicleCondition);
    }
}