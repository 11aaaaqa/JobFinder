using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Resume
{
    public class EmployeeExperienceDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Поле \"Должность\" обязательно")]
        [Display(Name = "Должность")]
        public string CompanyPost { get; set; }
        [Required(ErrorMessage = "Поле \"Компания\" обязательно")]
        [Display(Name = "Компания")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Поле \"Начало работы\" обязательно")]
        public string WorkingFrom { get; set; }
        [Required(ErrorMessage = "Поле \"Окончание работы\" обязательно")]
        public string WorkingUntil { get; set; }
        public TimeSpan WorkingDuration { get; set; } = TimeSpan.Zero;
        public string? Responsibilities { get; set; }
    }
}
