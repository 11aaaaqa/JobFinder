namespace EmployerMicroservice.Api.DTOs
{
    public class AddCompanyDto
    {
        public Guid FounderEmployerId { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyDescription { get; set; }
        public uint CompanyColleaguesCount { get; set; }
    }
}
