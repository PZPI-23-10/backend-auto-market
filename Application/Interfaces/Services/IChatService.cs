using Application.DTOs.Chat;

namespace Application.Interfaces.Services;

public interface IChatService
{
    Task<ChatDto> GetOrCreateChatAsync(int userA, int userB);
    Task<ChatMessageDto> SendMessageAsync(int chatId, int senderId, string text);
    Task<List<ChatMessageDto>> GetHistoryAsync(int chatId);
    Task<List<ChatDto>> GetUserChatsAsync(int userId);
}