using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleModelBodyTypeController(IVehicleModelBodyTypeRepository  vehicleModelBodyTypeRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vehicleModelBodyType = await vehicleModelBodyTypeRepository.GetAllAsync();
        return Ok(vehicleModelBodyType.Select(GetResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vehicleModelBodyType = await vehicleModelBodyTypeRepository.GetByIdAsync(id);

        if (vehicleModelBodyType == null)
            return NotFound();

        return Ok(GetResponse(vehicleModelBodyType));
    }
    
    private VehicleModelBodyTypeResponse GetResponse(VehicleModelBodyType vehicleModelBodyType)
    {
        var response = new VehicleModelBodyTypeResponse
        {
            Id = vehicleModelBodyType.Id,
            ModelName = vehicleModelBodyType.VehicleModel.Name,
            BodyName = vehicleModelBodyType.BodyType.Name
        };
        
        return response;
    }
}