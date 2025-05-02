namespace Web.MVC.DTOs.Resume
{
    public class EmployeeExperienceDto
    {
        public Guid Id { get; set; }
        public string CompanyPost { get; set; }
        public string CompanyName { get; set; }
        public string WorkingFrom { get; set; }
        public string WorkingUntil { get; set; }
        public TimeSpan WorkingDuration { get; set; } = TimeSpan.Zero;
        public string? Responsibilities { get; set; }
    }
}
