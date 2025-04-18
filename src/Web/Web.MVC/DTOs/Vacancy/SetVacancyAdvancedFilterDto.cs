namespace Web.MVC.DTOs.Vacancy
{
    public class SetVacancyAdvancedFilterDto
    {
        public string? Position { get; set; }
        public int? SalaryFrom { get; set; }
        public string? WorkExperience { get; set; }
        public string? EmploymentType { get; set; }
        public bool OfficeWorkType { get; set; }
        public bool RemoteWorkType { get; set; }
        public List<string>? VacancyCities { get; set; }
    }
}
