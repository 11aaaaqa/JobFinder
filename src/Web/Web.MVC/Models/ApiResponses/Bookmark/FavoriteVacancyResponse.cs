namespace Web.MVC.Models.ApiResponses.Bookmark
{
    public class FavoriteVacancyResponse
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid VacancyId { get; set; }
        public string Position { get; set; }
        public int? SalaryFrom { get; set; }
        public int? SalaryTo { get; set; }
        public string? WorkExperience { get; set; }
        public string CompanyName { get; set; }
        public string VacancyCity { get; set; }
    }
}
