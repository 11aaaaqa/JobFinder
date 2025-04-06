namespace VacancyMicroservice.Api.DTOs
{
    public class GetFilteredVacanciesDto
    {
        public string? Position { get; set; }
        public int? SalaryFrom { get; set; }
        public string? WorkExperience { get; set; }
        public string? EmploymentType { get; set; }
        public bool? RemoteWork { get; set; }
        public List<string>? VacancyCities { get; set; }
    }
}
