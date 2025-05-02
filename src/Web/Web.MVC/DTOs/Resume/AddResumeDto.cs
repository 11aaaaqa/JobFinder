using System.ComponentModel.DataAnnotations;
using Web.MVC.Models.ApiResponses.Resume;

namespace Web.MVC.DTOs.Resume
{
    public class AddResumeDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        [Required]
        public string ResumeTitle { get; set; }
        public List<string>? OccupationTypes { get; set; }
        public List<string>? WorkTypes { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? City { get; set; }
        public string Status { get; set; }
        public bool ReadyToMove { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        public string? AboutMe { get; set; }
        public uint? DesiredSalary { get; set; }
        public List<EducationDto>? Educations { get; set; } = new();
        public List<EmployeeExperienceDto>? EmployeeExperience { get; set; } = new();
        public List<ForeignLanguageDto>? ForeignLanguages { get; set; } = new();
    }
}
