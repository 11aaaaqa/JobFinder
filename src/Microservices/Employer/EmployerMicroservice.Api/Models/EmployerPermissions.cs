namespace EmployerMicroservice.Api.Models
{
    public class EmployerPermissions
    {
        public Guid Id { get; set; }
        public Guid EmployerId { get; set; }
        public List<string> Permissions { get; set; }
    }
}
