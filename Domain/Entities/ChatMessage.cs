using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class ChatMessage : BaseAuditableEntity
{
    public int ChatId { get; set; }
    public int SenderId { get; set; }

    public string Text { get; set; }
    
    public bool IsRead { get; set; }

    [ForeignKey(nameof(SenderId))] public virtual User Sender { get; set; }
    [ForeignKey(nameof(ChatId))] public virtual Chat Chat { get; set; }
}