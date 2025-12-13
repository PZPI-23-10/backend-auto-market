namespace Application.DTOs.Chat;

public class ChatDto
{
    public int Id { get; set; }
    public int FirstUserId { get; set; }
    public int SecondUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<ChatMessageDto> Messages { get; set; }
}