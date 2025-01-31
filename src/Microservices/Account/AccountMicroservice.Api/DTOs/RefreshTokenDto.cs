namespace AccountMicroservice.Api.DTOs
{
    public class RefreshTokenDto
    {
        public string ExpiredAccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}