using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Resume
{
    public class EducationDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Поле \"Уровень образования\" обязательно")]
        [Display(Name = "Уровень образования")]
        public string EducationType { get; set; }
        [Display(Name = "Образовательное учреждение")]
        public string? EducationalInstitution { get; set; }
        [Display(Name = "Специализация")]
        public string? Specialization { get; set; }
    }
}
