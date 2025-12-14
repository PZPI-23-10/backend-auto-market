using Api.Extensions;
using Application.DTOs.Chat;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatController(IChatService chatService) : ControllerBase
{
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<ChatDto>>> GetMyChats()
    {
        int userId = User.GetUserId();
        IEnumerable<ChatDto> chats = await chatService.GetUserChatsAsync(userId);
        return Ok(chats);
    }

    [HttpGet("{chatId:int}/history")]
    public async Task<ActionResult<ChatMessageDto>> GetHistory(int chatId)
    {
        var messages = await chatService.GetHistoryAsync(chatId);
        return Ok(messages);
    }

    [HttpPost("with/{otherUserId:int}")]
    public async Task<ActionResult<ChatDto>> GetOrCreateChat(int otherUserId)
    {
        int userId = User.GetUserId();

        var chat = await chatService.GetOrCreateChatAsync(userId, otherUserId);
        return Ok(chat);
    }

    [HttpGet("{chatId:int}/unreadCount")]
    public async Task<ActionResult<int>> Unread(int chatId)
    {
        int userId = User.GetUserId();

        return Ok(await chatService.GetUnreadCountAsync(chatId, userId));
    }
}