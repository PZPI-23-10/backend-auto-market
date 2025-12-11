using Application.DTOs.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IVehicleVerificationService
    {
        Task<VehicleCheckResultDto> CheckVehicleAsync(string identifier, string lang = "ua");
    }
}
