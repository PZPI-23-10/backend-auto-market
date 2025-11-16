namespace Application.DTOs.Location;

public class RegionResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<CityResponse> Cities { get; set; } = [];
}