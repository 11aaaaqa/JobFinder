using ChatMicroservice.Api.DTOs;
using ChatMicroservice.Api.Models;
using ChatMicroservice.Api.Services.Chat_services;
using ChatMicroservice.Api.Services.Message_services;
using GeneralLibrary.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ChatMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController(IMessageService messageService, IChatService chatService) : ControllerBase
    {
        [HttpGet]
        [Route("GetMessageByMessageId/{messageId}")]
        public async Task<IActionResult> GetMessageByMessageIdAsync(Guid messageId)
        {
            var message = await messageService.GetMessageByIdAsync(messageId);
            if (message is null)
                return NotFound();
            return Ok(message);
        }

        [HttpGet]
        [Route("GetLastMessagesByChatId/{chatId}")]
        public async Task<IActionResult> GetLastMessagesByChatIdAsync(Guid chatId, int pageNumber)
        {
            var messages = await messageService.GetLastMessagesByChatIdAsync(chatId, pageNumber);
            return Ok(new Queue<Message>(messages));
        }

        [HttpPost]
        [Route("CreateMessage")]
        public async Task<IActionResult> CreateMessageAsync([FromBody] CreateMessageDto model)
        {
            var chat = await chatService.GetChatByIdAsync(model.ChatId);
            if (chat is null) return NotFound();

            await messageService.CreateMessageAsync(new Message
            {
                ChatId = model.ChatId, CreatedAt = DateTime.UtcNow, Id = model.Id,
                Text = model.Text, SenderId = model.SenderId, IsRead = false
            });

            if (model.SenderId == chat.EmployeeId)
            {
                await chatService.IncreaseUnreadMessagesCountByOneAsync(model.ChatId, AccountTypeEnum.Employer);
            }
            else
            {
                await chatService.IncreaseUnreadMessagesCountByOneAsync(model.ChatId, AccountTypeEnum.Employee);
            }

            await chatService.UpdateLastMessageSendingTimeAsync(model.ChatId);

            return Ok();
        }

        [HttpGet]
        [Route("GetFirstUnreadMessages/{chatId}")]
        public async Task<IActionResult> GetFirstUnreadMessagesAsync(Guid chatId, AccountTypeEnum currentAccountType)
        {
            var chat = await chatService.GetChatByIdAsync(chatId);
            if (chat is null) return NotFound();

            List<Message> messages = new List<Message>();
            switch (currentAccountType)
            {
                case AccountTypeEnum.Employee:
                    messages = await messageService.GetFirstUnreadMessagesAsync(chatId, chat.EmployeeId, null);
                    break;
                case AccountTypeEnum.Employer:
                    messages = await messageService.GetFirstUnreadMessagesAsync(chatId, null, chat.EmployerId);
                    break;
            }
            return Ok(messages);
        }

        [HttpGet]
        [Route("GetLastReadMessages/{chatId}")]
        public async Task<IActionResult> GetLastReadMessagesAsync(Guid chatId, AccountTypeEnum currentAccountType, int pageNumber)
        {
            var chat = await chatService.GetChatByIdAsync(chatId);
            if (chat is null) return NotFound();

            List<Message> messages = new List<Message>();
            switch (currentAccountType)
            {
                case AccountTypeEnum.Employee:
                    messages = await messageService.GetLastReadMessagesAsync(chatId, null, chat.EmployeeId, pageNumber);
                    break;
                case AccountTypeEnum.Employer:
                    messages = await messageService.GetLastReadMessagesAsync(chatId, chat.EmployerId, null, pageNumber);
                    break;
            }

            return Ok(messages);
        }

        [HttpGet]
        [Route("MarkMessageAsRead/{messageId}")]
        public async Task<IActionResult> MarkMessageAsReadAsync(Guid messageId, Guid? currentEmployerId, Guid? currentEmployeeId)
        {
            if (currentEmployeeId == null && currentEmployerId == null)
                return BadRequest();

            var succeeded = await messageService.MarkMessageAsReadAsync(messageId);
            if (succeeded)
            {
                var message = await messageService.GetMessageByIdAsync(messageId);
                if (currentEmployeeId == null)
                {
                    await chatService.DecreaseUnreadMessagesCountAsync(message.ChatId, 1, AccountTypeEnum.Employer);
                }
                else
                {
                    await chatService.DecreaseUnreadMessagesCountAsync(message.ChatId, 1, AccountTypeEnum.Employee);
                }
                return Ok();
            }

            return NotFound();
        }

        [HttpPost]
        [Route("MarkMessagesAsRead")]
        public async Task<IActionResult> MarkMessagesAsReadAsync([FromBody] MarkMessagesAsReadDto model)
        {
            int markedAsReadCount = await messageService.MarkMessagesAsReadAsync(model.Messages);
            await chatService.DecreaseUnreadMessagesCountAsync(model.Messages[0].ChatId, markedAsReadCount, model.CurrentAccountType);

            return Ok();
        }
    }
}
