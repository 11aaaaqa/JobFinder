using System.Security.Claims;
using System.Text;
using System.Text.Json;
using GeneralLibrary.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.Employer;

namespace Web.MVC.Chat_services
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

        public async Task Send(string message, string to, Guid chatId)
        {
            Guid messageId = Guid.NewGuid();
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var accountType = Context.User.FindFirst(ClaimTypeConstants.AccountTypeClaimName).Value;
            Guid senderId;
            if (accountType == AccountTypeConstants.Employee)
            {
                var employeeResponse = await httpClient.GetAsync(
                    $"{url}/api/Employee/GetEmployeeByEmail?email={Context.User.Identity.Name}");
                employeeResponse.EnsureSuccessStatusCode();
                var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();
                senderId = employee.Id;
            }
            else
            {
                var employerResponse = await httpClient.GetAsync(
                        $"{url}/api/Employer/GetEmployerByEmail?email={Context.User.Identity.Name}");
                employerResponse.EnsureSuccessStatusCode();
                var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();
                senderId = employer.Id;
            }

            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                ChatId = chatId, SenderId = senderId, Text = message, Id = messageId
            }), Encoding.UTF8, "application/json");
            var addMessageResponse = await httpClient.PostAsync($"{url}/api/Message/CreateMessage", jsonContent);
            addMessageResponse.EnsureSuccessStatusCode();

            await Clients.User(to).SendAsync("ReceiveMessage", messageId, chatId, message, DateTime.UtcNow);
        }
    }
}
