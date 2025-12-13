using Application.DTOs.Vehicle;

namespace Application.Interfaces.Services
{
    public interface IVehicleVerificationService
    {
        Task<VehicleCheckResultDto> CheckVehicleAsync(string identifier, string lang = "ua");
    }
}
