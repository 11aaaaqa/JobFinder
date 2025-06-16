using ChatMicroservice.Api.DTOs;
using ChatMicroservice.Api.Models;
using ChatMicroservice.Api.Services.Chat_services;
using Microsoft.AspNetCore.Mvc;

namespace ChatMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(IChatService chatService) : ControllerBase
    {
        [HttpGet]
        [Route("GetChatByChatId/{chatId}")]
        public async Task<IActionResult> GetChatByChatIdAsync(Guid chatId)
        {
            var chat = await chatService.GetChatByIdAsync(chatId);
            if (chat is null)
                return NotFound();
            return Ok(chat);
        }

        [HttpGet]
        [Route("GetChatListByEmployeeId/{employeeId}")]
        public async Task<IActionResult> GetChatListByEmployeeIdAsync(Guid employeeId, string? searchingQuery, int pageNumber)
            => Ok(await chatService.GetChatListByEmployeeIdAsync(employeeId, searchingQuery, pageNumber));

        [HttpGet]
        [Route("GetChatListByEmployerId/{employerId}")]
        public async Task<IActionResult> GetChatListByEmployerIdAsync(Guid employerId, string? searchingQuery, int pageNumber)
            => Ok(await chatService.GetChatListByEmployerIdAsync(employerId, searchingQuery, pageNumber));

        [HttpPost]
        [Route("CreateChat")]
        public async Task<IActionResult> CreateChatAsync([FromBody] CreateChatDto model)
        {
            await chatService.CreateChatAsync(new Chat
            {
                EmployeeFullName = model.EmployeeFullName, EmployeeId = model.EmployeeId,
                EmployerFullName = model.EmployerFullName, EmployerId = model.EmployerId,
                Id = model.Id, LastMessageSendingTime = DateTime.UtcNow
            });

            return Ok();
        }
    }
}
