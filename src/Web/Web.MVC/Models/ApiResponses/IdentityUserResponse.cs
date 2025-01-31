namespace Web.MVC.Models.ApiResponses
{
    public class IdentityUserResponse
    {
        public string Id { get; set; }
        public string AccountType { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
