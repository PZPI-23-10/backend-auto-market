namespace Application.DTOs.Chat;

public class ChatDto
{
    public int Id { get; set; }
    public ChatUserDto FirstUser { get; set; }
    public ChatUserDto SecondUser { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public List<ChatMessageDto> Messages { get; set; }
}