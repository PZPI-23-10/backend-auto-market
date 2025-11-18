using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleBodyTypeController(
    IVehicleBodyTypeRepository vehicleBodyType, IVehicleModelRepository models) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleBodyTypeResponse>>> GetAll()
    {
        var bodyTypes = await vehicleBodyType.GetAllAsync();
        return Ok(bodyTypes.Select(GetResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehicleBodyTypeResponse>> GetById(int id)
    {
        var bodyType = await vehicleBodyType.GetByIdAsync(id);

        if (bodyType == null)
            return NotFound();

        return Ok(GetResponse(bodyType));
    }

    [HttpGet("for-model/{modelId:int}")]
    public async Task<ActionResult<IEnumerable<VehicleBodyTypeResponse>>> GetBodyTypes(int modelId)
    {
        var vehicleModel = await models.GetByIdAsync(modelId);

        if (vehicleModel == null)
            return NotFound();

        ICollection<VehicleModelBodyType> modelTypes = vehicleModel.VehicleModelBodyTypes;

        return Ok(modelTypes.Select(x => new VehicleBodyTypeResponse { Id = x.BodyType.Id, Name = x.BodyType.Name }));
    }
    
    private VehicleBodyTypeResponse GetResponse(VehicleBodyType entity)
    {
        var response = new VehicleBodyTypeResponse
        {
            Id = entity.Id,
            Name = entity.Name,
        };

        return response;
    }
}