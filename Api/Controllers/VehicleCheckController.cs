using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleCheckController(IVehicleVerificationService verificationService) : ControllerBase
{
    [HttpGet("vin/{vin}")]
    public async Task<IActionResult> CheckByVin(string vin, [FromQuery] string lang = "ua")
    {
        var result = await verificationService.CheckVehicleAsync(vin, lang);

        if (!result.IsFound)
            return NotFound(new { message = result.ErrorMessage ?? "Авто не знайдено" });

        return Ok(result);
    }

    [HttpGet("plate/{plate}")]
    public async Task<IActionResult> CheckByPlate(string plate, [FromQuery] string lang = "ua")
    {
        var result = await verificationService.CheckVehicleAsync(plate, lang);

        if (!result.IsFound)
            return NotFound(new { message = result.ErrorMessage ?? "Авто не знайдено" });

        return Ok(result);
    }
}