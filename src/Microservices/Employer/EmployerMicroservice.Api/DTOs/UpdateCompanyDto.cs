namespace EmployerMicroservice.Api.DTOs
{
    public class UpdateCompanyDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyDescription { get; set; }
        public uint CompanyColleaguesCount { get; set; }
    }
}
