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
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AboutMe { get; set; }
        public uint? DesiredSalary { get; set; }
        public List<EducationResponse>? Educations { get; set; }
        public List<EmployeeExperienceResponse>? EmployeeExperience { get; set; }
        public List<ForeignLanguageResponse>? ForeignLanguages { get; set; }
    }
}
