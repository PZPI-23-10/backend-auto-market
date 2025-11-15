using Domain.Enums;

namespace Domain.Entities;

public class Car : BaseAuditableEntity
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public string Description { get; set; }
    public DateTime Year { get; set; }
    public int Mileage { get; set; }
    public string PhotoUrl { get; set; }
    public GearboxType GearboxType { get; set; }
    public FuelType FuelType { get; set; }
    public decimal Price { get; set; }
}