namespace EmployeeMicroservice.Api.DTOs
{
    public class UpdateEmployeeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
