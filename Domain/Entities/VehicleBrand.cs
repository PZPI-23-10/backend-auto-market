namespace Domain.Entities;

public class VehicleBrand : BaseEntity
{
    public string Name { get; set; }

    public virtual ICollection<VehicleModel> VehicleModels { get; set; } = new List<VehicleModel>();
}