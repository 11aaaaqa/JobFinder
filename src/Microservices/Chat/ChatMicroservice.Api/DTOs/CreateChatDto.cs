namespace ChatMicroservice.Api.DTOs
{
    public class CreateChatDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }

        public Guid EmployerId { get; set; }
        public string EmployerFullName { get; set; }
    }
}
