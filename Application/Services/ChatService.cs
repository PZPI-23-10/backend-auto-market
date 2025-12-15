using Application.DTOs.Chat;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ChatService(IDataContext dataContext) : IChatService
{
    public async Task<IEnumerable<ChatDto>> GetUserChatsAsync(int userId)
    {
        var chats = await dataContext.Chats
            .Where(c => c.FirstUserId == userId || c.SecondUserId == userId)
            .Include(c => c.Messages)
            .ThenInclude(m => m.Reads.Where(r => r.UserId == userId))
            .ToListAsync();

        return chats.Select(c => ToDto(c, userId));
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
        });
    }

    public async Task<ChatDto> GetOrCreateChatAsync(int userA, int userB)
    {
        var chat = await dataContext.Chats
            .Include(c => c.Messages)
            .ThenInclude(m => m.Reads)
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
            };

            dataContext.Chats.Add(chat);
            await dataContext.SaveChangesAsync();
        }

        return ToDto(chat, userA);
    }

    public async Task<ChatMessageDto> SendMessageAsync(int chatId, int senderId, string text)
    {
        var message = new ChatMessage
        {
            ChatId = chatId,
            SenderId = senderId,
            Text = text,
        };

        dataContext.ChatMessages.Add(message);
        await dataContext.SaveChangesAsync();

        return new ChatMessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            Text = message.Text,
            SentAt = message.Created,
            IsRead = false
        };
    }

    public async Task<IEnumerable<ChatMessageDto>> GetHistoryAsync(int chatId, int currentUserId)
    {
        var messages = await dataContext.ChatMessages
            .Include(m => m.Reads.Where(r => r.UserId == currentUserId))
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.Created)
            .ToListAsync();

        return messages.Select(m => new ChatMessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            Text = m.Text,
            SentAt = m.Created,
            IsRead = m.Reads.Count != 0
        });
    }

    public async Task MarkChatAsReadAsync(int chatId, int readerId)
    {
        var unreadMessageIds = await dataContext.ChatMessages
            .Where(m => m.ChatId == chatId && m.SenderId != readerId)
            .Select(m => m.Id)
            .ToListAsync();

        var alreadyRead = await dataContext.ChatMessageReads
            .Where(r => r.UserId == readerId && unreadMessageIds.Contains(r.MessageId))
            .Select(r => r.MessageId)
            .ToListAsync();

        var newReads = unreadMessageIds
            .Except(alreadyRead)
            .Select(id => new ChatMessageRead
            {
                MessageId = id,
                UserId = readerId
            });

        dataContext.ChatMessageReads.AddRange(newReads);
        await dataContext.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(int chatId, int userId)
    {
        return await dataContext.ChatMessages
            .Where(m =>
                m.ChatId == chatId &&
                m.SenderId != userId && m.Reads.All(r => r.UserId != userId))
            .CountAsync();
    }

    private ChatDto ToDto(Chat chat, int userId)
    {
        return new ChatDto
        {
            Id = chat.Id,
            FirstUser = new ChatUserDto
            {
                Id = chat.FirstUserId,
                FirstName = chat.FirstUser.FirstName,
                LastName = chat.FirstUser.LastName ?? string.Empty,
                PhotoUrl = chat.FirstUser.Avatar != null ? chat.FirstUser.Avatar.Url : string.Empty,
            },
            SecondUser = new ChatUserDto
            {
                Id = chat.SecondUserId,
                FirstName = chat.SecondUser.FirstName,
                LastName = chat.SecondUser.LastName ?? string.Empty,
                PhotoUrl = chat.SecondUser.Avatar != null ? chat.SecondUser.Avatar.Url : string.Empty,
            },
            CreatedAt = chat.Created,
            Messages = chat.Messages?
                .OrderBy(m => m.Created)
                .Select(message => new ChatMessageDto
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    Text = message.Text,
                    SentAt = message.Created,
                    IsRead = message.Reads.Any(r => r.UserId == userId)
                })
                .ToList() ?? []
        };
    }
}