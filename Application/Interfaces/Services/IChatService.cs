using Application.DTOs.Chat;

namespace Application.Interfaces.Services;

public interface IChatService
{
    Task<ChatDto> GetOrCreateChatAsync(int userA, int userB);
    Task<ChatMessageDto> SendMessageAsync(int chatId, int senderId, string text);
    Task<IEnumerable<ChatMessageDto>> GetHistoryAsync(int chatId);
    Task<IEnumerable<ChatDto>> GetUserChatsAsync(int userId);
}