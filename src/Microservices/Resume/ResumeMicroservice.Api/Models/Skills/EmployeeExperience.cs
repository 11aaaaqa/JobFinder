namespace ResumeMicroservice.Api.Models.Skills
{
    public class EmployeeExperience
    {
        public Guid Id { get; set; }
        public string CompanyPost { get; set; }
        public string CompanyName { get; set; }
        public string WorkingFrom { get; set; }
        public string WorkingUntil { get; set; }
        public bool CurrentlyWorkHere { get; set; }
        public TimeSpan WorkingDuration { get; set; }
        public string? Responsibilities { get; set; }
    }
}
