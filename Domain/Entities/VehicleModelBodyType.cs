using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class VehicleModelBodyType
{
    public int VehicleModelId { get; set; }
    [ForeignKey(nameof(VehicleModelId))] public VehicleModel VehicleModel { get; set; }

    public int BodyTypeId { get; set; }
    [ForeignKey(nameof(BodyTypeId))] public VehicleBodyType BodyType { get; set; }
}