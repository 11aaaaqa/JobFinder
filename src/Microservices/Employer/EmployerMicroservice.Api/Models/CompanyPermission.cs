namespace EmployerMicroservice.Api.Models
{
    public class CompanyPermission
    {
        public Guid Id { get; set; }
        public Guid EmployerId { get; set; }
        public Guid CompanyId { get; set; }
        public List<string> Permissions { get; set; }
    }
}
