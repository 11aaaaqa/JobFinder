using ChatMicroservice.Api.Constants;
using ChatMicroservice.Api.Database;
using ChatMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatMicroservice.Api.Services.Chat
{
    public class ChatService(ApplicationDbContext context) : IChatService
    {
        public async Task<List<ChatModel>> GetChatListByEmployeeIdAsync(Guid employeeId, int pageNumber)
        {
            var chats = await context.Chats
                .Where(x => x.EmployeeId == employeeId)
                .Skip((pageNumber - 1) * PaginationConstants.ChatsPageSize)
                .Take(PaginationConstants.ChatsPageSize)
                .ToListAsync();
            return chats;
        }

        public async Task<List<ChatModel>> GetChatListByEmployerIdAsync(Guid employerId, int pageNumber)
        {
            var chats = await context.Chats
                .Where(x => x.EmployerId == employerId)
                .Skip((pageNumber - 1) * PaginationConstants.ChatsPageSize)
                .Take(PaginationConstants.ChatsPageSize)
                .ToListAsync();
            return chats;
        }

        public async Task CreateChatAsync(ChatModel chat)
        {
            await context.Chats.AddAsync(chat);
            await context.SaveChangesAsync();
        }
    }
}
