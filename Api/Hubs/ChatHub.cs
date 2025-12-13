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
        int senderId = int.Parse(Context.UserIdentifier);

        var messageDto = await chatService.SendMessageAsync(chatId, senderId, text);

        await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", messageDto);
    }
}