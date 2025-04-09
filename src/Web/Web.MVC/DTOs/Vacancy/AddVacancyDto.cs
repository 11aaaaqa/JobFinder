using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Vacancy
{
    public class AddVacancyDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        [Required]
        public string Position { get; set; }
        public int? SalaryFrom { get; set; }
        public int? SalaryTo { get; set; }
        public string? WorkExperience { get; set; }
        public string EmploymentType { get; set; }
        public bool RemoteWork { get; set; }
        [Required]
        public string VacancyCity { get; set; }
        [Required]
        public string Address { get; set; }
        [StringLength(1500)]
        public string? WorkerResponsibilities { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string? EmployerContactPhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? EmployerContactEmail { get; set; }
    }
}
