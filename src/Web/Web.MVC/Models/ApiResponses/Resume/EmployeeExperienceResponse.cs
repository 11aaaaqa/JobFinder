namespace Web.MVC.Models.ApiResponses.Resume
{
    public class EmployeeExperienceResponse
    {
        public Guid Id { get; set; }
        public string CompanyPost { get; set; }
        public string CompanyName { get; set; }
        public string WorkingFrom { get; set; }
        public string WorkingUntil { get; set; }
        public TimeSpan WorkingDuration { get; set; }
        public string? Responsibilities { get; set; }
    }
}
