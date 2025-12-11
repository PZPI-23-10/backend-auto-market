namespace Application.DTOs.Vehicle;

public class VehicleCheckResultDto
{
    public bool IsFound { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Digits { get; set; } 
    public string? Vin { get; set; }   
    public bool IsStolen { get; set; }
    public string? Color { get; set; }
    public string? Fuel { get; set; }
    public int EngineCapacity { get; set; }
    public string? LastOperationDate { get; set; }
    public string? LastOperationName { get; set; }
    public string? ErrorMessage { get; set; }
}