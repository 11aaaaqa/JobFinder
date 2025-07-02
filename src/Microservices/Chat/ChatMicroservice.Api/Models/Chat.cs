using System.Text.Json.Serialization;

namespace ChatMicroservice.Api.Models
{
    public class Chat
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }
        public int EmployeeUnreadMessagesCount { get; set; } = 0;

        public Guid EmployerId { get; set; }
        public string EmployerFullName { get; set; }
        public int EmployerUnreadMessagesCount { get; set; } = 0;
        public DateTime LastMessageSendingTime { get; set; }

        [JsonIgnore]
        public List<Message> Messages { get; set; } = new();
    }
}
