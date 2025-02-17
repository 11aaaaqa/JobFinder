namespace CompanyMicroservice.Api.DTOs
{
    public class UpdateCompanyDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyDescription { get; set; }
        public string CompanyColleaguesCount { get; set; }
    }
}
