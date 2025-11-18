using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleConditionController(IVehicleConditionRepository vehicleConditionRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vehicleConditions = await vehicleConditionRepository.GetAllAsync();
        return Ok(vehicleConditions.Select(GetResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vehicleCondition = await vehicleConditionRepository.GetByIdAsync(id);

        if (vehicleCondition == null)
            return NotFound();

        return Ok(GetResponse(vehicleCondition));
    }

    private VehicleConditionResponse GetResponse(VehicleCondition entity)
    {
        return new VehicleConditionResponse
        {
            Id = entity.Id,
            Name = entity.Name,
        };
    }
}