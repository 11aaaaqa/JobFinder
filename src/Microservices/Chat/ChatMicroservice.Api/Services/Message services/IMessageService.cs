using ChatMicroservice.Api.Models;

namespace ChatMicroservice.Api.Services.Message_services
{
    public interface IMessageService
    {
        Task<Message?> GetMessageByIdAsync(Guid messageId);
        Task<List<Message>> GetLastMessagesByChatIdAsync(Guid chatId, int pageNumber);
        Task CreateMessageAsync(Message message);
        Task<List<Message>> GetFirstUnreadMessagesAsync(Guid chatId, Guid? currentEmployerId, Guid? currentEmployeeId);
        Task<List<Message>> GetLastReadMessagesAsync(Guid chatId, Guid? currentEmployerId, Guid? currentEmployeeId, int pageNumber);
        Task<int> MarkMessagesAsReadAsync(List<Message> messages);
        Task<bool> MarkMessageAsReadAsync(Guid messageId);
    }
}
