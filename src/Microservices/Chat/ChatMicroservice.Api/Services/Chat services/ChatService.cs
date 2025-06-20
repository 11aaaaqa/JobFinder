using ChatMicroservice.Api.Constants;
using ChatMicroservice.Api.Database;
using ChatMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatMicroservice.Api.Services.Chat_services
{
    public class ChatService(ApplicationDbContext context) : IChatService
    {
        public async Task<Chat?> GetChatByIdAsync(Guid chatId)
            => await context.Chats.SingleOrDefaultAsync(x => x.Id == chatId);

        public async Task<List<Chat>> GetChatListByEmployeeIdAsync(Guid employeeId, string? searchingQuery, int pageNumber)
        {
            var chats = context.Chats.Where(x => x.EmployeeId == employeeId).AsQueryable();

            if (searchingQuery is not null)
                chats = chats.Where(x => x.EmployerFullName.ToLower().Contains(searchingQuery.ToLower()));

            return await chats.OrderByDescending(x => x.LastMessageSendingTime)
                .Skip((pageNumber - 1) * PaginationConstants.ChatsPageSize)
                .Take(PaginationConstants.ChatsPageSize)
                .ToListAsync(); ;
        }

        public async Task<List<Chat>> GetChatListByEmployerIdAsync(Guid employerId, string? searchingQuery, int pageNumber)
        {
            var chats = context.Chats.Where(x => x.EmployerId == employerId).AsQueryable();

            if (searchingQuery is not null)
                chats = chats.Where(x => x.EmployeeFullName.ToLower().Contains(searchingQuery.ToLower()));

            return await chats.OrderByDescending(x => x.LastMessageSendingTime)
                .Skip((pageNumber - 1) * PaginationConstants.ChatsPageSize)
                .Take(PaginationConstants.ChatsPageSize)
                .ToListAsync(); ;
        }

        public async Task CreateChatAsync(Chat chat)
        {
            await context.Chats.AddAsync(chat);
            await context.SaveChangesAsync();
        }

        public async Task UpdateLastMessageSendingTimeAsync(Guid chatId)
        {
            var chat = await context.Chats.SingleOrDefaultAsync(x => x.Id == chatId);
            if(chat is null)
                return;

            chat.LastMessageSendingTime = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }

        public async Task<Chat?> GetChatAsync(Guid employeeId, Guid employerId)
        {
            var chat = await context.Chats.Where(x => x.EmployeeId == employeeId)
                .SingleOrDefaultAsync(x => x.EmployerId == employerId);
            return chat;
        }
    }
}
