namespace EmployerMicroservice.Api.DTOs
{
    public class UpdateEmployerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? CompanyPost { get; set; }
    }
}
