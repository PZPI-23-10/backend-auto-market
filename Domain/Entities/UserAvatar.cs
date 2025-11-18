using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class UserAvatar : BaseEntity
{
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))] public virtual User User { get; set; }

    public string Url { get; set; }
    public string? Hash { get; set; }
    public string? PublicId { get; set; }
    public bool IsExternal { get; set; }
}