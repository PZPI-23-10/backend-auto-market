using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuelTypeController(IFuelTypeRepository fuelTypes, IUnitOfWork unitOfWork) : ControllerBase
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
    
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] string name)
    {
        if (await fuelTypes.Exists(name))
            return BadRequest("Vehicle brand with that name already exists");

        var fuelType = new FuelType { Name = name };
        await fuelTypes.AddAsync(fuelType);

        await unitOfWork.SaveChangesAsync();

        return Ok(fuelType.Id);
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var fuelType = await fuelTypes.GetByIdAsync(id);

        if (fuelType == null)
            return BadRequest("Vehicle brand with that id does not exist");

        fuelTypes.Remove(fuelType);
        await unitOfWork.SaveChangesAsync();

        return Ok();
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