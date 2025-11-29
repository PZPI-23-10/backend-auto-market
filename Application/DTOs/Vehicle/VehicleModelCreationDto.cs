namespace Application.DTOs.Vehicle;

public class VehicleModelCreationDto
{
    public string Name { get; set; }
    public int VehicleBrandId { get; set; }
    public int VehicleBodyTypeId { get; set; }
    public int VehicleTypeId { get; set; }
}