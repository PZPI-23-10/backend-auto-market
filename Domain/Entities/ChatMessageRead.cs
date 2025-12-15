using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class ChatMessageRead
{
    public int MessageId { get; set; }
    public int UserId { get; set; }

    public DateTimeOffset ReadAt { get; set; } = DateTimeOffset.Now;

    [ForeignKey(nameof(MessageId))] public virtual ChatMessage Message { get; set; }
}