using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class VehicleListing : BaseAuditableEntity
{
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))] public virtual User? User { get; set; }

    public int? ModelId { get; set; }
    [ForeignKey(nameof(ModelId))] public virtual VehicleModel? Model { get; set; }

    public int? GearTypeId { get; set; }
    [ForeignKey(nameof(GearTypeId))] public virtual GearType? GearType { get; set; }

    public int? BodyTypeId { get; set; }
    [ForeignKey(nameof(BodyTypeId))] public virtual VehicleBodyType? BodyType { get; set; }

    public int? ConditionId { get; set; }
    [ForeignKey(nameof(ConditionId))] public virtual VehicleCondition? Condition { get; set; }

    public int? FuelTypeId { get; set; }
    [ForeignKey(nameof(FuelTypeId))] public virtual FuelType? FuelType { get; set; }

    [MaxLength(7)] public string? ColorHex { get; set; }

    public int? CityId { get; set; }
    [ForeignKey(nameof(CityId))] public virtual City? City { get; set; }

    public int? Year { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    public int? Mileage { get; set; }
    public bool? HasAccident { get; set; }
    public decimal? Price { get; set; }
    public string? Number { get; set; }
    public bool IsPublished { get; set; } = false;
    public string? Vin { get; set; }
    public bool IsVerified { get; set; } = false;
    public virtual ICollection<VehiclePhoto> Photos { get; set; } = new List<VehiclePhoto>();
}