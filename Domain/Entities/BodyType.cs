using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class BodyType : BaseEntity
{
    public int VehicleModelId { get; set; }
    [ForeignKey(nameof(VehicleModelId))] public VehicleModel VehicleModel { get; set; }
}