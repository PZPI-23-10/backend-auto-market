using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleBrandController(IVehicleBrandRepository vehicleBrand) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var brand = await vehicleBrand.GetAllAsync();
        return Ok(brand.Select(GetVehicleBrandResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var brand = await vehicleBrand.GetByIdAsync(id);

        if (brand == null)
            return NotFound();

        return Ok(GetVehicleBrandResponse(brand));
    }

    private VehicleBrandResponse GetVehicleBrandResponse(VehicleBrand brand)
    {
        var response = new VehicleBrandResponse
        {
            Id = brand.Id,
            Name = brand.Name,
        };
        
        return response;
    }
}