namespace Domain.Entities;

public class Region : BaseEntity
{
    public string Name { get; set; }

    public ICollection<City> Cities { get; set; } = new List<City>();
}