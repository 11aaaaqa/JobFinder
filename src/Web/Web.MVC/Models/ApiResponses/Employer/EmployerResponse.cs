namespace Web.MVC.Models.ApiResponses.Employer
{
    public class EmployerResponse
    {
        public Guid Id { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string? CompanyPost { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
