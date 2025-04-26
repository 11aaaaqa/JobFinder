using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Vacancy
{
    public class EditVacancyDto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Поле \"Должность\" обязательно")]
        public string Position { get; set; }
        public int? SalaryFrom { get; set; }
        public int? SalaryTo { get; set; }
        public string? WorkExperience { get; set; }
        [Required(ErrorMessage = "Поле \"Занятость\" обязательно")]
        public string EmploymentType { get; set; }
        public bool RemoteWork { get; set; }
        [Required(ErrorMessage = "Поле \"Город\" обязательно")]
        public string VacancyCity { get; set; }
        public string? Address { get; set; }
        public string Description { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string? EmployerContactPhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? EmployerContactEmail { get; set; }
    }
}
