namespace CompanyMicroservice.Api.Models
{
    public class InvitationRequestedEmployers
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid EmployerId { get; set; }
        public string EmployerName { get; set; }
        public string EmployerSurname { get; set; }
        public DateTime JoiningRequestDate { get; set; }
    }
}
