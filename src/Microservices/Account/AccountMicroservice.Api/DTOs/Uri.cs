namespace AccountMicroservice.Api.DTOs
{
    public class Uri
    {
        public string Scheme { get; set; }
        public string DomainName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}