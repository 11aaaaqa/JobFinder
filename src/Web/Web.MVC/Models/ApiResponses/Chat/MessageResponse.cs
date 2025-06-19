namespace Web.MVC.Models.ApiResponses.Chat
{
    public class MessageResponse
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public ChatResponse Chat { get; set; }
        public Guid SenderId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
