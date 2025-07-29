using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Models.ApiResponses.Notification;

namespace Web.MVC.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;
        public NotificationController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        }

        [Route("notifications/get-by-email/json")]
        public async Task<IActionResult> GetJsonNotificationsByUserEmail(string userEmail)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var notificationsResponse = await httpClient.GetAsync(
                $"{url}/api/Notification/GetNotificationsByUserEmail?userEmail={userEmail}&pageNumber=1");
            notificationsResponse.EnsureSuccessStatusCode();
            var notifications = await notificationsResponse.Content.ReadFromJsonAsync<List<NotificationResponse>>();

            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                Notifications = notifications
            }), Encoding.UTF8, "application/json");
            var removeNotificationsResponse = await httpClient.PostAsync($"{url}/api/Notification/RemoveNotifications", jsonContent);
            removeNotificationsResponse.EnsureSuccessStatusCode();

            return new JsonResult(notifications);
        }

        [Route("notifications/remove")]
        [HttpPost]
        public async Task<IActionResult> RemoveNotifications([FromBody] List<NotificationResponse> notifications)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                Notifications = notifications
            }), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{url}/api/Notification/RemoveNotifications", jsonContent);
            response.EnsureSuccessStatusCode();

            return Ok();
        }

        [Route("notifications/get-notifications-count-by-user-email")]
        public async Task<IActionResult> GetJsonNotificationsCountByUserEmail(string userEmail)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"{url}/api/Notification/GetNotificationsCountByUserEmail?userEmail={userEmail}");
            response.EnsureSuccessStatusCode();
            int notificationsCount = await response.Content.ReadFromJsonAsync<int>();

            return new JsonResult(notificationsCount);
        }

        [Route("notifications/{notificationId}")]
        public async Task<IActionResult> GetNotificationByIdJson(Guid notificationId)
        {
            using HttpClient httpsClient = httpClientFactory.CreateClient();

            var notificationResponse = await httpsClient.GetAsync($"{url}/api/Notification/GetNotificationById/{notificationId}");
            notificationResponse.EnsureSuccessStatusCode();
            var notification = await notificationResponse.Content.ReadFromJsonAsync<NotificationResponse>();

            return new JsonResult(notification);
        }
    }
}
