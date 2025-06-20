namespace Web.MVC.Models.ApiResponses.Chat
{
    public class ChatResponse
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }

        public Guid EmployerId { get; set; }
        public string EmployerFullName { get; set; }
        public DateTime LastMessageSendingTime { get; set; }

        public List<MessageResponse> Messages { get; set; } = new();
    }
}
