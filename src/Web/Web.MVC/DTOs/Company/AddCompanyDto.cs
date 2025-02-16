using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Company
{
    public class AddCompanyDto
    {
        public Guid FounderEmployerId { get; set; }

        [Required(ErrorMessage = "Поле \"Название компании\" обязательно")]
        [Display(Name = "Название компании")]
        [StringLength(50)]
        public string CompanyName { get; set; }

        [Display(Name = "Описание")]
        public string? CompanyDescription { get; set; }

        [Required(ErrorMessage = "Поле \"Количество сотрудников\" обязательно")]
        [Display(Name = "Количество сотрудников")]
        public string CompanyColleaguesCount { get; set; }
    }
}
