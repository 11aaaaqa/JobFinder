using EmployeeMicroservice.Api.Models.EmployeeSkills;

namespace EmployeeMicroservice.Api.Models
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public bool ReadyToMove { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Post { get; set; }
        public uint? DesiredSalaryFrom { get; set; }
        public uint? DesiredSalaryTo { get; set; }
        public string? OccupationType { get; set; }
        public string? AboutMe { get; set; }
        public List<Education> Educations { get; set; }
        public List<EmployeeExperience> EmployeeExperience { get; set; }
        public List<ForeignLanguage> ForeignLanguages { get; set; }
    }
}
