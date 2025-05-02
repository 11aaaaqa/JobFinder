using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Resume
{
    public class EmployeeExperienceDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [Display(Name = "Должность")]
        public string CompanyPost { get; set; }
        [Required]
        [Display(Name = "Компания")]
        public string CompanyName { get; set; }
        [Required]
        public string WorkingFrom { get; set; }
        [Required]
        public string WorkingUntil { get; set; }
        public TimeSpan WorkingDuration { get; set; } = TimeSpan.Zero;
        public string? Responsibilities { get; set; }
    }
}
