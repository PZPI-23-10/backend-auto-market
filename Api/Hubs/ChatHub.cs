using Api.Extensions;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub(IChatService chatService) : Hub
{
    public Task JoinChat(int chatId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }

    public async Task SendMessage(int chatId, string text)
    {
        int senderId = Context.User.GetUserId();

        var messageDto = await chatService.SendMessageAsync(chatId, senderId, text);

        await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", messageDto);
    }

    public async Task MarkAsRead(int chatId)
    {
        int readerId = Context.User.GetUserId();

        await chatService.MarkChatAsReadAsync(chatId, readerId);

        await Clients.Group(chatId.ToString())
            .SendAsync("MessagesRead", chatId, readerId);
    }
}