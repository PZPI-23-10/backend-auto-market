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
            .FirstOrDefaultAsync(c =>
                (c.FirstUserId == userA && c.SecondUserId == userB) ||
                (c.FirstUserId == userB && c.SecondUserId == userA));

        if (chat == null)
        {
            chat = new Chat
            {
                FirstUserId = userA,
                SecondUserId = userB,
                Messages = new List<ChatMessage>()
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
        };

        dataContext.ChatMessages.Add(message);
        await dataContext.SaveChangesAsync();

        return new ChatMessageDto()
        {
            Id = message.Id,
            SenderId = message.SenderId,
            Text = message.Text,
            SentAt = message.Created
        };
    }

    public async Task<IEnumerable<ChatMessageDto>> GetHistoryAsync(int chatId)
    {
        var chatMessages = await dataContext.ChatMessages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.Created)
            .ToListAsync();

        return chatMessages.Select(m => new ChatMessageDto()
        {
            Id = m.Id,
            SenderId = m.SenderId,
            Text = m.Text,
            SentAt = m.Created
        });
    }

    public async Task<IEnumerable<ChatDto>> GetUserChatsAsync(int userId)
    {
        var chats = await dataContext.Chats
            .Where(c => c.FirstUserId == userId || c.SecondUserId == userId)
            .ToListAsync();

        return chats.Select(ToDto);
    }

    private ChatDto ToDto(Chat chat)
    {
        return new ChatDto
        {
            Id = chat.Id,
            FirstUserId = chat.FirstUserId,
            SecondUserId = chat.SecondUserId,
            CreatedAt = chat.Created,
            Messages = chat.Messages
                .OrderBy(m => m.Created)
                .Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    Text = m.Text,
                    SentAt = m.Created
                })
                .ToList()
        };
    }
}