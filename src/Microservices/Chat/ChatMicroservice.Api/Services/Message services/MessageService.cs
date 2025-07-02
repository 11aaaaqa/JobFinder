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

        public async Task<List<Message>> GetFirstUnreadMessagesAsync(Guid chatId, Guid? currentEmployerId, Guid? currentEmployeeId)
        {
            var currentId = currentEmployeeId ?? currentEmployerId;
            if (currentId == null)
                throw new ArgumentException("Current employee and employer IDs cannot be both null");

            var messages = await context.Messages
                .Where(x => x.ChatId == chatId)
                .Where(x => x.IsRead == false && x.SenderId != currentId)
                .OrderBy(x => x.CreatedAt)
                .Take(PaginationConstants.MessagesPageSize)
                .ToListAsync();
            return messages;
        }

        public async Task<List<Message>> GetLastReadMessagesAsync(Guid chatId, Guid? currentEmployerId,
            Guid? currentEmployeeId, int pageNumber)
        {
            var currentId = currentEmployeeId ?? currentEmployerId;
            if (currentId == null)
                throw new ArgumentException("Current employee and employer IDs cannot be both null");

            var messages = await context.Messages
                .Where(x => x.ChatId == chatId)
                .Where(x => x.IsRead == true || x.SenderId == currentId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * PaginationConstants.MessagesPageSize)
                .Take(PaginationConstants.MessagesPageSize)
                .ToListAsync();
            return messages;
        }

        public async Task<int> MarkMessagesAsReadAsync(List<Message> messages)
        {
            int counter = 0;
            var messageIdsToUpdate = messages.Where(x => !x.IsRead).Select(x => x.Id).ToList();
            var messagesToUpdate = await context.Messages.Where(x => messageIdsToUpdate.Contains(x.Id)).ToListAsync();
            foreach (var message in messagesToUpdate)
            {
                message.IsRead = true;
                counter++;
            }

            await context.SaveChangesAsync();
            return counter;
        }

        public async Task<bool> MarkMessageAsReadAsync(Guid messageId)
        {
            var message = await context.Messages.SingleOrDefaultAsync(x => x.Id == messageId);
            if(message == null)
                return false;
            
            message.IsRead = true;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
