namespace EmployerMicroservice.Api.Kafka.Consumer_Models
{
    public class EmployerJoinedCompanyModel
    {
        public Guid EmployerId { get; set; }
        public Guid CompanyId { get; set; }
    }
}
