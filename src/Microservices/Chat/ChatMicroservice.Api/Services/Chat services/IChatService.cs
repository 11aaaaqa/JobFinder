using ChatMicroservice.Api.Models;

namespace ChatMicroservice.Api.Services.Chat_services
{
    public interface IChatService
    {
        Task<Chat?> GetChatByIdAsync(Guid chatId);
        Task<List<Chat>> GetChatListByEmployeeIdAsync(Guid employeeId, int pageNumber);
        Task<List<Chat>> GetChatListByEmployerIdAsync(Guid employerId, int pageNumber);
        Task CreateChatAsync(Chat chat);
        Task UpdateLastMessageSendingTimeAsync(Guid chatId);
    }
}
