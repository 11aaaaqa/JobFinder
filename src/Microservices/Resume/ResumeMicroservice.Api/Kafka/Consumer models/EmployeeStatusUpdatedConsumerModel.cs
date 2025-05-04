namespace ResumeMicroservice.Api.Kafka.Consumer_models
{
    public class EmployeeStatusUpdatedConsumerModel
    {
        public Guid EmployeeId { get; set; }
        public string Status { get; set; }
    }
}
