using ChatMicroservice.Api.DTOs;
using ChatMicroservice.Api.Models;
using ChatMicroservice.Api.Services.Chat_services;
using ChatMicroservice.Api.Services.Message_services;
using Microsoft.AspNetCore.Mvc;

namespace ChatMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController(IMessageService messageService, IChatService chatService) : ControllerBase
    {
        [HttpGet]
        [Route("GetLastMessagesByChatId/{chatId}")]
        public async Task<IActionResult> GetLastMessagesByChatIdAsync(Guid chatId, int pageNumber)
            => Ok(await messageService.GetLastMessagesByChatIdAsync(chatId, pageNumber));

        [HttpPost]
        [Route("CreateMessage")]
        public async Task<IActionResult> CreateMessageAsync([FromBody] CreateMessageDto model)
        {
            await messageService.CreateMessageAsync(new Message
            {
                ChatId = model.ChatId, CreatedAt = DateTime.UtcNow, Id = Guid.NewGuid(),
                Text = model.Text, SenderId = model.SenderId
            });

            await chatService.UpdateLastMessageSendingTimeAsync(model.ChatId);

            return Ok();
        }
    }
}
