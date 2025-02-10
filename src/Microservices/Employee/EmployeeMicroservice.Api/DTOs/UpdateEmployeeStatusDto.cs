namespace EmployeeMicroservice.Api.DTOs
{
    public class UpdateEmployeeStatusDto
    {
        public Guid EmployeeId { get; set; }
        public string Status { get; set; }
    }
}
