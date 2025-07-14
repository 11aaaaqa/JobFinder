using NotificationMicroservice.Api.Models;

namespace NotificationMicroservice.Api.Services
{
    public interface INotificationService
    {
        public Task<Notification?> GetNotificationByIdAsync(Guid id);
        public Task<List<Notification>> GetNotificationsByUserId(string aspNetUserId, int pageNumber);
        public Task AddNotificationAsync(Notification notification);
        public Task RemoveNotificationsAsync(List<Notification> notifications);
    }
}
