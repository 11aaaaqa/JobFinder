namespace CompanyMicroservice.Api.DTOs
{
    public class RequestJoiningCompanyDto
    {
        public Guid CompanyId { get; set; }
        public Guid EmployerId { get; set; }
        public string EmployerName { get; set; }
        public string EmployerSurname { get; set; }
    }
}
