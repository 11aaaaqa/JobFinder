using Microsoft.AspNetCore.Identity;

namespace AccountMicroservice.Api.Models
{
    public class User : IdentityUser
    {
        public string AccountType { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}