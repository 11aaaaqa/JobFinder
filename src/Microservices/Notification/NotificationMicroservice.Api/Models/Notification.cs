namespace NotificationMicroservice.Api.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string AspNetUserId { get; set; } 
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
