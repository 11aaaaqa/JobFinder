using System.ComponentModel.DataAnnotations;

namespace EmployerMicroservice.Api.Models
{
    public class EmployerPermissions
    {
        [Key]
        public Guid EmployerId { get; set; }
        public List<string> Permissions { get; set; }
    }
}
