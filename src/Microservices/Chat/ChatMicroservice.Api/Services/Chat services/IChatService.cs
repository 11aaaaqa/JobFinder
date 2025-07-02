using ChatMicroservice.Api.Models;
using GeneralLibrary.Enums;

namespace ChatMicroservice.Api.Services.Chat_services
{
    public interface IChatService
    {
        Task<Chat?> GetChatByIdAsync(Guid chatId);
        Task<List<Chat>> GetChatListByEmployeeIdAsync(Guid employeeId, string? searchingQuery, int pageNumber);
        Task<List<Chat>> GetChatListByEmployerIdAsync(Guid employerId, string? searchingQuery, int pageNumber);
        Task CreateChatAsync(Chat chat);
        Task UpdateLastMessageSendingTimeAsync(Guid chatId);
        Task<Chat?> GetChatAsync(Guid employeeId, Guid employerId);
        Task IncreaseUnreadMessagesCountByOneAsync(Guid chatId, AccountTypeEnum accountType);
        Task DecreaseUnreadMessagesCountAsync(Guid chatId, int count, AccountTypeEnum accountType);
        Task<int> GetMessagesCountByChatAsync(Guid chatId);
    }
}
