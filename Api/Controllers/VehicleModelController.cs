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
    public async Task<ActionResult<IEnumerable<VehicleModelResponse>>> GetFiltered(
        [FromQuery] int? vehicleModelId,
        [FromQuery] int? brandId, 
        [FromQuery] int? vehicleTypeId)
    {
        var vehicleModels = (await vehicleModelRepository.GetByBrandAndTypeAsync(brandId, vehicleTypeId, vehicleModelId)).ToList();

        var response = vehicleModels.Select(vm => GetResponse(vm));

        return Ok(response);
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