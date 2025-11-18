using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class VehiclePhoto : BaseEntity
{
    public int VehicleListingId { get; set; }
    [ForeignKey(nameof(VehicleListingId))] public virtual VehicleListing VehicleListing { get; set; }

    public string PhotoUrl { get; set; }
    public string PublicId { get; set; }
    public string Hash { get; set; }
}