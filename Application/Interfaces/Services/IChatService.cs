using Application.DTOs.Chat;

namespace Application.Interfaces.Services;

public interface IChatService
{
    Task<ChatDto> GetOrCreateChatAsync(int userA, int userB);
    Task<ChatMessageDto> SendMessageAsync(int chatId, int senderId, string text);
    Task<IEnumerable<ChatMessageDto>> GetHistoryAsync(int chatId, int currentUserId);
    Task<IEnumerable<ChatDto>> GetUserChatsAsync(int userId);
    Task MarkChatAsReadAsync(int chatId, int readerId);
    Task<int> GetUnreadCountAsync(int chatId, int userId);
}