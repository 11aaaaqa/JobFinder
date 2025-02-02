namespace EmployeeMicroservice.Api.Kafka.Consumer_models
{
    public class AccountConsumerModel
    {
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AccountType { get; set; }
        public string Email { get; set; }
    }
}