using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class VehicleListing : BaseAuditableEntity
{
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))] public User User { get; set; }

    public int BrandId { get; set; }
    [ForeignKey(nameof(BrandId))] public VehicleBrand Brand { get; set; }

    public int ModelId { get; set; }
    [ForeignKey(nameof(ModelId))] public VehicleModel Model { get; set; }

    public int BodyTypeId { get; set; }
    [ForeignKey(nameof(BodyTypeId))] public BodyType BodyType { get; set; }

    public int ConditionId { get; set; }
    [ForeignKey(nameof(ConditionId))] public VehicleCondition Condition { get; set; }

    public int ColorId { get; set; }
    [ForeignKey(nameof(ColorId))] public VehicleColor Color { get; set; }

    public int CityId { get; set; }
    [ForeignKey(nameof(CityId))] public City City { get; set; }

    public DateTime Year { get; set; }
    [MaxLength(2000)] public string Description { get; set; }
    public int Mileage { get; set; }
    public bool HasAccident { get; set; }
    public decimal Price { get; set; }

    public ICollection<VehiclePhoto> Photos { get; set; } = new List<VehiclePhoto>();
}