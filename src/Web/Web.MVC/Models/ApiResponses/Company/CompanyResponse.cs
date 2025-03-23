namespace Web.MVC.Models.ApiResponses.Company
{
    public class CompanyResponse
    {
        public Guid Id { get; set; }
        public Guid FounderEmployerId { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyDescription { get; set; }
        public string CompanyColleaguesCount { get; set; }
    }
}
