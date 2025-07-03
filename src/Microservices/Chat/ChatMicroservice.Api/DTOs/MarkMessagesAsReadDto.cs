using ChatMicroservice.Api.Models;
using GeneralLibrary.Enums;

namespace ChatMicroservice.Api.DTOs
{
    public class MarkMessagesAsReadDto
    {
        public List<Message> Messages { get; set; }
        public AccountTypeEnum CurrentAccountType { get; set; }
    }
}
