using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class VehiclePhoto : BaseEntity
{
    public int CarId { get; set; }
    [ForeignKey(nameof(CarId))] public VehicleListing VehicleListing { get; set; }

    public string PhotoUrl { get; set; }
}