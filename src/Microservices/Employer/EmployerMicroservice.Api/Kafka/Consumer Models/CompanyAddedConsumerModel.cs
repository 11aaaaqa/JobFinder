namespace EmployerMicroservice.Api.Kafka.Consumer_Models
{
    public class CompanyAddedConsumerModel
    {
        public Guid CompanyId { get; set; }
        public Guid EmployerId { get; set; }
    }
}
