using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Employee
{
    public class UpdateEmployeeDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Поле \"Имя\" обязательно")]
        [Display(Name = "Имя")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле \"Фамилия\" обязательно")]
        [Display(Name = "Фамилия")]
        [StringLength(50)]
        public string Surname { get; set; }

        [Display(Name = "Отчество")]
        [StringLength(50)]
        public string? Patronymic { get; set; }

        [Display(Name = "Пол")]
        public string? Gender { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }

        [Display(Name = "Город проживания")]
        [StringLength(50)]
        public string? City { get; set; }

        [Display(Name = "Номер телефона")]
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
    }
}
