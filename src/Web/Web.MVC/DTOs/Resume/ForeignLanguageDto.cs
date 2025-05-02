using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Resume
{
    public class ForeignLanguageDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [Display(Name = "Язык")]
        public string LanguageName { get; set; }
        [Required]
        [Display(Name = "Уровень владения")]
        public string LanguageProficiencyLevel { get; set; }
    }
}
