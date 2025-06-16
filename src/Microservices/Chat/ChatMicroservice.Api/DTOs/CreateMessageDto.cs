namespace ChatMicroservice.Api.DTOs
{
    public class CreateMessageDto
    {
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
