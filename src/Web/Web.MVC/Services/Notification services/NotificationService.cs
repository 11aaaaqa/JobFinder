using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Web.MVC.Chat_services;
using Web.MVC.Services.Hub_connection_services;

namespace Web.MVC.Services.Notification_services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHubConnectionsManager hubConnectionsManager;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly string url;
        public NotificationService(IHttpClientFactory httpClientFactory, ILogger<NotificationService> logger, IConfiguration configuration,
            IHubConnectionsManager hubConnectionsManager, IHubContext<ChatHub> hubContext)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            this.hubConnectionsManager = hubConnectionsManager;
            this.hubContext = hubContext;
            url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        }
        public async Task AddNotification(string receiverUserEmail, string body)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            Guid notificationId = Guid.NewGuid();
            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                Id = notificationId,
                UserEmail = receiverUserEmail,
                Body = body
            }), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{url}/api/Notification/AddNotification", jsonContent);
            if (!response.IsSuccessStatusCode)
                logger.LogCritical("Something went wrong, notification hasn't been created");

            var connection = hubConnectionsManager.GetConnection(receiverUserEmail);
            if (connection != null)
                await hubContext.Clients.Client(connection).SendAsync("ReceiveNotification", notificationId);
        }
    }
}
