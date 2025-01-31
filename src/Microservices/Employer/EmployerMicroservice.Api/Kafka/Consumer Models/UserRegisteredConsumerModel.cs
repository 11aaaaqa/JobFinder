namespace EmployerMicroservice.Api.Kafka.Consumer_Models
{
    public class UserRegisteredConsumerModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AccountType { get; set; }
        public string Email { get; set; }
    }
}