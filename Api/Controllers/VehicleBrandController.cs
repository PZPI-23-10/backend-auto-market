using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleBrandController(IVehicleBrandRepository vehicleBrands, IVehicleTypeRepository vehicleTypes)
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