using System.Text;
using System.Text.Json;
using GeneralLibrary.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.Chat;
using Web.MVC.Models.ApiResponses.Employer;

namespace Web.MVC.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;
        public ChatHub(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        }

        public async Task Send(string message, Guid chatId)
        {
            Guid messageId = Guid.NewGuid();
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var accountType = Context.User.FindFirst(ClaimTypeConstants.AccountTypeClaimName).Value;
            Guid senderId, receiverUserId;

            var chatResponse = await httpClient.GetAsync($"{url}/api/Chat/GetChatByChatId/{chatId}");
            chatResponse.EnsureSuccessStatusCode();
            var chat = await chatResponse.Content.ReadFromJsonAsync<ChatResponse>();
            if (accountType == AccountTypeConstants.Employee)
            {
                var employeeResponse = await httpClient.GetAsync(
                    $"{url}/api/Employee/GetEmployeeByEmail?email={Context.User.Identity.Name}");
                employeeResponse.EnsureSuccessStatusCode();
                var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();
                senderId = employee.Id;
                receiverUserId = chat.EmployerId;
            }
            else
            {
                var employerResponse = await httpClient.GetAsync(
                    $"{url}/api/Employer/GetEmployerByEmail?email={Context.User.Identity.Name}");
                employerResponse.EnsureSuccessStatusCode();
                var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();
                senderId = employer.Id;
                receiverUserId = chat.EmployeeId;
            }

            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                ChatId = chatId, SenderId = senderId, Text = message, Id = messageId
            }), Encoding.UTF8, "application/json");
            var addMessageResponse = await httpClient.PostAsync($"{url}/api/Message/CreateMessage", jsonContent);
            addMessageResponse.EnsureSuccessStatusCode();

            await Clients.Group($"chat_{chatId}")
                .SendAsync("ReceiveMessage", messageId, message, DateTime.UtcNow, senderId);
            await Clients.Group($"chats_list_{receiverUserId}").SendAsync("ReceiveMessage", messageId, chatId);
        }

        public async Task AddToChatGroup(string hubConnection, Guid chatId)
        {
            await Groups.AddToGroupAsync(hubConnection, $"chat_{chatId}");
        }

        public async Task AddToChatsListGroup(string hubConnection, Guid userId)
        {
            await Groups.AddToGroupAsync(hubConnection, $"chats_list_{userId}");
        }
    }
}
