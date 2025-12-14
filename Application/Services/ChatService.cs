using Application.DTOs.Chat;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ChatService(IDataContext dataContext) : IChatService
{
    public async Task<ChatDto> GetOrCreateChatAsync(int userA, int userB)
    {
        var chat = await dataContext.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c =>
                (c.FirstUserId == userA && c.SecondUserId == userB) ||
                (c.FirstUserId == userB && c.SecondUserId == userA));

        if (chat == null)
        {
            chat = new Chat
            {
                FirstUserId = userA,
                SecondUserId = userB,
                Messages = new List<ChatMessage>(),
                Created = DateTimeOffset.UtcNow 
            };

            dataContext.Chats.Add(chat);
            await dataContext.SaveChangesAsync();
        }

        return ToDto(chat);
    }

    public async Task<ChatMessageDto> SendMessageAsync(int chatId, int senderId, string text)
    {
        var message = new ChatMessage
        {
            ChatId = chatId,
            SenderId = senderId,
            Text = text,
            IsRead = false,
            Created = DateTimeOffset.UtcNow 
        };

        dataContext.ChatMessages.Add(message);
        await dataContext.SaveChangesAsync();

        return new ChatMessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            Text = message.Text,
            SentAt = message.Created,
            IsRead = message.IsRead
        };
    }

    public async Task<IEnumerable<ChatMessageDto>> GetHistoryAsync(int chatId)
    {
        var chatMessages = await dataContext.ChatMessages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.Created)
            .ToListAsync();

        return chatMessages.Select(message => new ChatMessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            Text = message.Text,
            SentAt = message.Created,
            IsRead = message.IsRead
        });
    }

    public async Task<IEnumerable<ChatDto>> GetUserChatsAsync(int userId)
    {
        var chats = await dataContext.Chats
            .Include(c => c.Messages)
            .Where(c => c.FirstUserId == userId || c.SecondUserId == userId)
            .ToListAsync();

        return chats.Select(ToDto);
    }

    public async Task MarkChatAsReadAsync(int chatId, int readerId)
    {
        var unread = await dataContext.ChatMessages
            .Where(message =>
                message.ChatId == chatId &&
                message.SenderId != readerId &&
                !message.IsRead)
            .ToListAsync();

        if (unread.Any())
        {
            foreach (var msg in unread)
                msg.IsRead = true;

            await dataContext.SaveChangesAsync();
        }
    }

    public async Task<int> GetUnreadCountAsync(int chatId, int userId)
    {
        return await dataContext.ChatMessages.CountAsync(m =>
            m.ChatId == chatId &&
            m.SenderId != userId &&
            !m.IsRead);
    }

    private ChatDto ToDto(Chat chat)
    {
        return new ChatDto
        {
            Id = chat.Id,
            FirstUserId = chat.FirstUserId,
            SecondUserId = chat.SecondUserId,
            CreatedAt = chat.Created,
            Messages = chat.Messages?
                .OrderBy(m => m.Created)
                .Select(message => new ChatMessageDto
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    Text = message.Text,
                    SentAt = message.Created,
                    IsRead = message.IsRead
                })
                .ToList() ?? new List<ChatMessageDto>() 
        };
    }
}