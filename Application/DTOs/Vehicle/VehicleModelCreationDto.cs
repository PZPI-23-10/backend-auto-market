namespace Application.DTOs.Vehicle;

public class VehicleModelCreationDto
{
    public string Name { get; set; }
    public int VehicleBrandId { get; set; }
    public List<int> VehicleBodyTypesIds { get; set; }
    public int VehicleTypeId { get; set; }
}