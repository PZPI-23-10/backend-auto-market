namespace Domain.Entities;

public class VehicleType : BaseEntity
{
    public string Name { get; set; }

    public virtual ICollection<VehicleModel> Models { get; set; }
}