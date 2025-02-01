using ResumeMicroservice.Api.Models.Skills;

namespace ResumeMicroservice.Api.Models
{
    public class Resume
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string ResumeTitle { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? City { get; set; }
        public string Status { get; set; }
        public string Specialization { get; set; }
        public bool ReadyToMove { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AboutMe { get; set; }
        public uint? DesiredSalaryFrom { get; set; }
        public uint? DesiredSalaryTo { get; set; }
        public uint? DesiredSalary { get; set; }
        public List<Education> Educations { get; set; }
        public List<EmployeeExperience> EmployeeExperience { get; set; }
        public List<ForeignLanguage> ForeignLanguages { get; set; }
    }
}
