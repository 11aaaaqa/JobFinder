namespace ChatMicroservice.Api.Models
{
    public class Chat
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }

        public Guid EmployerId { get; set; }
        public string EmployerFullName { get; set; }
        public DateTime LastMessageSendingTime { get; set; }
        public int UnreadMessagesCount { get; set; }

        public List<Message> Messages { get; set; } = new();
    }
}
