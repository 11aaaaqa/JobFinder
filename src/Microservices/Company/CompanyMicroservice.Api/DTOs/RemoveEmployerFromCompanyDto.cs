namespace CompanyMicroservice.Api.DTOs
{
    public class RemoveEmployerFromCompanyDto
    {
        public Guid CompanyId { get; set; }
        public Guid EmployerId { get; set; }
    }
}
