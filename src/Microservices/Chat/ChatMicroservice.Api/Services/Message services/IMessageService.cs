using ChatMicroservice.Api.Models;

namespace ChatMicroservice.Api.Services.Message_services
{
    public interface IMessageService
    {
        Task<Message?> GetMessageByIdAsync(Guid messageId);
        Task<List<Message>> GetLastMessagesByChatIdAsync(Guid chatId, int pageNumber);
        Task CreateMessageAsync(Message message);
    }
}
