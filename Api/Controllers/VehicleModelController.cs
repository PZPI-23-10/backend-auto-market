using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleModelController(IVehicleModelRepository vehicleModelRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleModelResponse>>> GetAll()
    {
        var vehicleModel = await vehicleModelRepository.GetAllAsync();
        return Ok(vehicleModel.Select(GetResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehicleModelResponse>> GetById(int id)
    {
        var vehicleModel = await vehicleModelRepository.GetByIdAsync(id);

        if (vehicleModel == null)
            return NotFound();

        return Ok(GetResponse(vehicleModel));
    }

    private VehicleModelResponse GetResponse(VehicleModel vehicleModel)
    {
        var response = new VehicleModelResponse
        {
            Id = vehicleModel.Id,
            Name = vehicleModel.Name,
        };

        return response;
    }
}