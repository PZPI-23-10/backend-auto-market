using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class FavouriteVehicle : BaseEntity
{
    [ForeignKey(nameof(User))] 
    public int UserId { get; set; }
    public virtual User User { get; set; } 

    [ForeignKey(nameof(FavVehicleListing))] 
    public int VehicleListingId { get; set; }
    public virtual VehicleListing FavVehicleListing { get; set; }
}