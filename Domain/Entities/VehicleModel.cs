using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class VehicleModel : BaseEntity
{
    public int BrandId { get; set; }
    [ForeignKey(nameof(BrandId))] public VehicleBrand Brand { get; set; }
    public string Name { get; set; }
}