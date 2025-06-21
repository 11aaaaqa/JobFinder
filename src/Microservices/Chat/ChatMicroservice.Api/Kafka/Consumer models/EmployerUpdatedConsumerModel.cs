namespace ChatMicroservice.Api.Kafka.Consumer_models
{
    public class EmployerUpdatedConsumerModel
    {
        public Guid EmployerId { get; set; }
        public string NewName { get; set; }
        public string NewSurname { get; set; }
    }
}
