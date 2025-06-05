namespace ResponseMicroservice.Api.Models
{
    public class VacancyResponse
    {
        public Guid Id { get; set; }
        public Guid VacancyCompanyId { get; set; }
        public Guid EmployeeId { get; set; }
        public string ResponseStatus { get; set; }
        public DateTime ResponseDate { get; set; }

        public Guid RespondedEmployeeResumeId { get; set; }
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
    }
}
