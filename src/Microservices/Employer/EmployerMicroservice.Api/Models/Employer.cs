namespace EmployerMicroservice.Api.Models
{
    public class Employer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string? CompanyPost { get; set; }
        public Guid? CompanyId { get; set; }
    }
}