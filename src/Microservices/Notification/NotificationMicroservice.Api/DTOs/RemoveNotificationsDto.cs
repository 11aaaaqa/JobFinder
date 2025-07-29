using NotificationMicroservice.Api.Models;

namespace NotificationMicroservice.Api.DTOs
{
    public class RemoveNotificationsDto
    {
        public List<Notification> Notifications { get; set; }
    }
}
