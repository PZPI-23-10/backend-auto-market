using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class City : BaseEntity
{
    public int RegionId { get; set; }
    [ForeignKey(nameof(RegionId))] public virtual Region Region { get; set; }

    public string Name { get; set; }
}