namespace EmployeeMicroservice.Api.Models.EmployeeSkills
{
    public class EmployeeExperience
    {
        public Guid Id { get; set; }
        public string CompanyPost { get; set; }
        public string CompanyName { get; set; }
        public string WorkingFromMonth { get; set; }
        public string WorkingUntilMonth { get; set; }
        public string WorkingFromYear { get; set; }
        public string WorkingUntilYear { get; set; }
        public bool CurrentlyWorkHere { get; set; }
        public string? Responsibilities { get; set; }
    }
}
