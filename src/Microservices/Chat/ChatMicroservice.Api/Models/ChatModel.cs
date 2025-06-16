namespace ChatMicroservice.Api.Models
{
    public class ChatModel
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }

        public Guid EmployerId { get; set; }
        public string EmployerFullName { get; set; }

        public List<Message> Messages { get; set; }
    }
}
