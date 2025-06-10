namespace Web.MVC.Models.ApiResponses.Response
{
    public class InterviewInvitationResponse
    {
        public Guid EmployeeResumeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeSurname { get; set; }
        public uint? EmployeeDesiredSalary { get; set; }
        public DateOnly? EmployeeDateOfBirth { get; set; }
        public TimeSpan EmployeeWorkingExperience { get; set; }
        public string? EmployeeCity { get; set; }

        public Guid VacancyId { get; set; }
        public string VacancyPosition { get; set; }
        public int? VacancySalaryFrom { get; set; }
        public int? VacancySalaryTo { get; set; }
        public string? VacancyWorkExperience { get; set; }
        public string VacancyCity { get; set; }
        public string VacancyCompanyName { get; set; }

        public bool IsClosed { get; set; }
    }
}
