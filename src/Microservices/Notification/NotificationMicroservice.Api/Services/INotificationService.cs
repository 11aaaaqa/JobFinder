using NotificationMicroservice.Api.Models;

namespace NotificationMicroservice.Api.Services
{
    public interface INotificationService
    {
        public Task<Notification?> GetNotificationByIdAsync(Guid id);
        public Task<List<Notification>> GetNotificationsByUserEmailAsync(string userEmail, int pageNumber);
        public Task<int> GetNotificationsCountByUserEmailAsync(string userEmail);
        public Task AddNotificationAsync(Notification notification);
        public Task RemoveNotificationsAsync(List<Notification> notifications);
    }
}
