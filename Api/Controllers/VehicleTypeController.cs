using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;


namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleTypeController(IVehicleTypeRepository vehicleTypes, IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleTypeResponse>>> GetAll()
    {
        IEnumerable<VehicleType> vehicleModel = await vehicleTypes.GetAllAsync();
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
    
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] string name)
    {
        if (await vehicleTypes.Exists(name))
            return BadRequest("Vehicle brand with that name already exists");

        var types = new VehicleType { Name = name };
        await vehicleTypes.AddAsync(types);

        await unitOfWork.SaveChangesAsync();

        return Ok(types.Id);
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var types = await vehicleTypes.GetByIdAsync(id);

        if (types == null)
            return BadRequest("Vehicle brand with that id does not exist");

        vehicleTypes.Remove(types);
        await unitOfWork.SaveChangesAsync();

        return Ok();
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