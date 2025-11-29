using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleBrandController(
    IVehicleBrandRepository vehicleBrands,
    IVehicleTypeRepository vehicleTypes,
    IUnitOfWork unitOfWork)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleBrandResponse>>> GetAll()
    {
        var brand = await vehicleBrands.GetAllAsync();
        return Ok(brand.Select(GetResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehicleBrandResponse>> GetById(int id)
    {
        var brand = await vehicleBrands.GetByIdAsync(id);

        if (brand == null)
            return NotFound();

        return Ok(GetResponse(brand));
    }

    [HttpGet("for-type/{typeId:int}")]
    public async Task<ActionResult<VehicleBrandResponse>> GetByTypeId(int typeId)
    {
        var vehicleType = await vehicleTypes.GetByIdAsync(typeId);

        if (vehicleType == null)
            return NotFound();

        IQueryable<VehicleBrand> brandsQuery = vehicleBrands.Query()
            .Where(brand => brand.VehicleModels.Any(model => model.VehicleTypeId == typeId));

        List<VehicleBrand> availableBrands = await brandsQuery.ToListAsync();

        return Ok(availableBrands.Select(GetResponse));
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] string name)
    {
        if (await vehicleBrands.Exists(name))
            return BadRequest("Vehicle brand with that name already exists");

        var brand = new VehicleBrand { Name = name };
        await vehicleBrands.AddAsync(brand);

        await unitOfWork.SaveChangesAsync();

        return Ok(brand.Id);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var brand = await vehicleBrands.GetByIdAsync(id);

        if (brand == null)
            return BadRequest("Vehicle brand with that id does not exist");

        vehicleBrands.Remove(brand);
        await unitOfWork.SaveChangesAsync();

        return Ok();
    }

    private VehicleBrandResponse GetResponse(VehicleBrand brand)
    {
        var response = new VehicleBrandResponse
        {
            Id = brand.Id,
            Name = brand.Name,
        };

        return response;
    }
}