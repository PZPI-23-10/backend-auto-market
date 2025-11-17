using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class VehicleModelBodyType
{
    public int VehicleModelId { get; set; }
    [ForeignKey(nameof(VehicleModelId))] public virtual VehicleModel VehicleModel { get; set; }

    public int BodyTypeId { get; set; }
    [ForeignKey(nameof(BodyTypeId))] public virtual VehicleBodyType BodyType { get; set; }
}