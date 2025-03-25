namespace Web.MVC.DTOs.Company
{
    public class UpdateEmployerCompanyPermissionsDto
    {
        public Guid EmployerId { get; set; }
        public List<string> Permissions { get; set; }
    }
}
