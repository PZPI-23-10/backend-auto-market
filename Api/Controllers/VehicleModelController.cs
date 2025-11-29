using Application.DTOs.Vehicle;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleModelController(IVehicleModelRepository vehicleModelRepository, 
    IVehicleBrandRepository brandRepository,
    IVehicleBodyTypeRepository bodyTypeRepository,
    IVehicleTypeRepository typeRepository,
    IUnitOfWork unitOfWork) : ControllerBase
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
    
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] VehicleModelCreationDto dto)
    {
        if (await vehicleModelRepository.Exists(dto.Name))
            return BadRequest("Vehicle model with that name already exists");
        
        if (!await brandRepository.ExistsById(dto.VehicleBrandId))
            return BadRequest($"Vehicle brand with id {dto.VehicleBrandId} does not exist.");

        if (!await bodyTypeRepository.ExistsById(dto.VehicleBodyTypeId))
            return BadRequest($"Vehicle body type with id {dto.VehicleBodyTypeId} does not exist.");

        if (!await typeRepository.ExistsById(dto.VehicleTypeId))
            return BadRequest($"Vehicle type with id {dto.VehicleTypeId} does not exist.");
        
        var model = new VehicleModel
        {
            Name = dto.Name,
            
            //TODO Я структуру бд не знаю, сори
            //VehicleBrandId = dto.VehicleBrandId,
            //VehicleBodyTypeId = dto.VehicleBodyTypeId,
            VehicleTypeId = dto.VehicleTypeId
        };

        await vehicleModelRepository.AddAsync(model);
        await unitOfWork.SaveChangesAsync();

        return Ok(model.Id);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var model = await vehicleModelRepository.GetByIdAsync(id);

        if (model == null)
            return BadRequest("Vehicle model with that id does not exist");
        try
        {
            vehicleModelRepository.Remove(model);
            await unitOfWork.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            return BadRequest("Cannot delete vehicle model because it is associated with existing vehicles.");
        }

        return Ok();
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