namespace Web.MVC.Models.ApiResponses.Employee
{
    public class InterviewInvitationResponse
    {
        public Guid Id { get; set; }
        public Guid InvitedCompanyId { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime InvitationDate { get; set; }

        public Guid EmployeeResumeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeSurname { get; set; }
        public DateOnly? EmployeeDateOfBirth { get; set; }
        public TimeSpan EmployeeWorkingExperience { get; set; }
        public string? EmployeeCity { get; set; }

        public Guid VacancyId { get; set; }
        public string VacancyPosition { get; set; }
        public int? VacancySalaryFrom { get; set; }
        public int? VacancySalaryTo { get; set; }
        public string? VacancyWorkExperience { get; set; }
        public string VacancyCity { get; set; }
    }
}
