namespace CompanyMicroservice.Api.Models
{
    public class Company
    {
        public Guid Id { get; set; }
        public Guid FounderEmployerId { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyDescription { get; set; }
        public string CompanyColleaguesCount { get; set; }
        public List<Guid> CompanyEmployersIds { get; set; }
    }
}