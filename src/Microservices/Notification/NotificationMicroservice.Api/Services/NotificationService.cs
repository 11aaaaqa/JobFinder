using Microsoft.EntityFrameworkCore;
using NotificationMicroservice.Api.Constants;
using NotificationMicroservice.Api.Database;
using NotificationMicroservice.Api.Models;

namespace NotificationMicroservice.Api.Services
{
    public class NotificationService(ApplicationDbContext context) : INotificationService
    {
        public async Task<Notification?> GetNotificationByIdAsync(Guid id)
            => await context.Notifications.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<List<Notification>> GetNotificationsByUserEmailAsync(string userEmail, int pageNumber)
        {
            var notifications = await context.Notifications
                .Where(x => x.UserEmail == userEmail)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * PaginationConstants.NotificationsPageSize)
                .Take(PaginationConstants.NotificationsPageSize)
                .ToListAsync();
            return notifications;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();
        }

        public async Task RemoveNotificationsAsync(List<Notification> notifications)
        {
            var notificationsToDelete = await context.Notifications
                .Where(x => notifications.Contains(x))
                .ToListAsync();

            context.Notifications.RemoveRange(notificationsToDelete);
            await context.SaveChangesAsync();
        }
    }
}

