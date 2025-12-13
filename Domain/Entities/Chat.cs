using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Chat : BaseAuditableEntity
{
    public int FirstUserId { get; set; }
    public int SecondUserId { get; set; }

    [ForeignKey(nameof(FirstUserId))] public virtual User FirstUser { get; set; }
    [ForeignKey(nameof(SecondUserId))] public virtual User SecondUser { get; set; }

    public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}