using ChatMicroservice.Api.Constants;
using ChatMicroservice.Api.Database;
using ChatMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatMicroservice.Api.Services.Message_services
{
    public class MessageService(ApplicationDbContext context) : IMessageService
    {
        public async Task<Message?> GetMessageByIdAsync(Guid messageId)
            => await context.Messages.SingleOrDefaultAsync(x => x.Id == messageId);

        public async Task<List<Message>> GetLastMessagesByChatIdAsync(Guid chatId, int pageNumber)
        {
            var messages = await context.Messages
                .Where(x => x.ChatId == chatId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * PaginationConstants.MessagesPageSize)
                .Take(PaginationConstants.MessagesPageSize)
                .ToListAsync();
            return messages;
        }

        public async Task CreateMessageAsync(Message message)
        {
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();
        }
    }
}
