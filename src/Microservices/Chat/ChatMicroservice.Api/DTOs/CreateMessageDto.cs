namespace ChatMicroservice.Api.DTOs
{
    public class CreateMessageDto
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public string Text { get; set; }
    }
}
