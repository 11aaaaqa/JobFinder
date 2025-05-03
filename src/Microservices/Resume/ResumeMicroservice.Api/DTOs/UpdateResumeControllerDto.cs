using ResumeMicroservice.Api.Models.Skills;

namespace ResumeMicroservice.Api.DTOs
{
    public class UpdateResumeControllerDto
    {
        public Guid Id { get; set; }
        public string ResumeTitle { get; set; }
        public List<string>? OccupationTypes { get; set; }
        public List<string>? WorkTypes { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? City { get; set; }
        public bool ReadyToMove { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AboutMe { get; set; }
        public uint? DesiredSalary { get; set; }
        public List<Education>? Educations { get; set; }
        public List<EmployeeExperience>? EmployeeExperience { get; set; }
        public List<ForeignLanguage>? ForeignLanguages { get; set; }
    }
}
