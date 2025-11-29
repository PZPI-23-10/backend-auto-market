using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GearTypeController(IGearTypeRepository gearTypes, UnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GearTypeResponse>>> GetAll()
    {
        var vehicleModel = await gearTypes.GetAllAsync();
        return Ok(vehicleModel.Select(GetResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GearTypeResponse>> GetById(int id)
    {
        var vehicleModel = await gearTypes.GetByIdAsync(id);

        if (vehicleModel == null)
            return NotFound();

        return Ok(GetResponse(vehicleModel));
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] string name)
    {
        if (await gearTypes.Exists(name))
            return BadRequest("Vehicle brand with that name already exists");

        var gearType = new GearType { Name = name };
        await gearTypes.AddAsync(gearType);

        await unitOfWork.SaveChangesAsync();

        return Ok(gearType.Id);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var gearType = await gearTypes.GetByIdAsync(id);

        if (gearType == null)
            return BadRequest("Vehicle brand with that id does not exist");

        gearTypes.Remove(gearType);
        await unitOfWork.SaveChangesAsync();

        return Ok();
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