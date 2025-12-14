namespace Application.DTOs.Chat;

public class ChatMessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string Text { get; set; }
    public DateTimeOffset SentAt { get; set; }
    public bool IsRead { get; set; }
}