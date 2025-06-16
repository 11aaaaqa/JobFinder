using ChatMicroservice.Api.Models;

namespace ChatMicroservice.Api.Services.Chat
{
    public interface IChatService
    {
        Task<List<ChatModel>> GetChatListByEmployeeIdAsync(Guid employeeId, int pageNumber);
        Task<List<ChatModel>> GetChatListByEmployerIdAsync(Guid employerId, int pageNumber);
        Task CreateChatAsync(ChatModel chat);
    }
}
