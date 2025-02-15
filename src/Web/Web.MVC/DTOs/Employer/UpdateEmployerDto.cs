using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Employer
{
    public class UpdateEmployerDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Поле \"Имя\" обязательно")]
        [Display(Name = "Имя")]
        [StringLength(30, ErrorMessage = "Максимальная длина поля \"Имя\" - 30 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле \"Фамилия\" обязательно")]
        [Display(Name = "Фамилия")]
        [StringLength(30, ErrorMessage = "Максимальная длина поля \"Фамилия\" - 30 символов")]
        public string Surname { get; set; }

        [Display(Name = "Должность")]
        [StringLength(50, ErrorMessage = "Максимальная длина поля \"Должность\" - 50 символов")]
        public string? CompanyPost { get; set; }
    }
}
