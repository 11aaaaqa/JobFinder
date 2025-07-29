namespace NotificationMicroservice.Api.DTOs
{
    public class AddNotificationDto
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; }
        public string Body { get; set; }
    }
}
