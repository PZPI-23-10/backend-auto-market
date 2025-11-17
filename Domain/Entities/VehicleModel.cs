using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class VehicleModel : BaseEntity
{
    public int VehicleTypeId { get; set; }
    [ForeignKey(nameof(VehicleTypeId))] public virtual VehicleType VehicleType { get; set; }

    public int BrandId { get; set; }
    [ForeignKey(nameof(BrandId))] public virtual VehicleBrand Brand { get; set; }
    public string Name { get; set; }

    public virtual ICollection<VehicleModelBodyType> VehicleModelBodyTypes { get; set; } =
        new List<VehicleModelBodyType>();
}