namespace Web.MVC.Services.Notification_services
{
    public interface INotificationService
    {
        public Task AddNotification(string receiverUserEmail, string body);
    }
}
