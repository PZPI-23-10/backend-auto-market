using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleConditionController(
    IVehicleConditionRepository vehicleConditionRepository,
    IUnitOfWork unitOfWork
    ) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleConditionResponse>>> GetAll()
    {
        var vehicleConditions = await vehicleConditionRepository.GetAllAsync();
        return Ok(vehicleConditions.Select(GetResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehicleConditionResponse>> GetById(int id)
    {
        var vehicleCondition = await vehicleConditionRepository.GetByIdAsync(id);

        if (vehicleCondition == null)
            return NotFound();

        return Ok(GetResponse(vehicleCondition));
    }
    
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] string name)
    {
        if (await vehicleConditionRepository.Exists(name))
            return BadRequest("Vehicle condition with that name already exists");

        var brand = new VehicleCondition() { Name = name };
        await vehicleConditionRepository.AddAsync(brand);

        await unitOfWork.SaveChangesAsync();

        return Ok(brand.Id);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var condition = await vehicleConditionRepository.GetByIdAsync(id);

        if (condition == null)
            return BadRequest("Vehicle brand with that id does not exist");

        vehicleConditionRepository.Remove(condition);
        await unitOfWork.SaveChangesAsync();

        return Ok();
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