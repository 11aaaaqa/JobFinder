namespace Web.MVC.Models.ApiResponses.Notification
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
