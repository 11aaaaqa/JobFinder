namespace ResumeMicroservice.Api.Kafka.Consumer_models
{
    public class EmployeeUpdatedConsumerModel
    {
        public Guid EmployeeId { get; set; }
        public string NewName { get; set; }
        public string NewSurname { get; set; }
        public string? NewPatronymic { get; set; }
        public string? NewGender { get; set; }
        public DateOnly? NewDateOfBirth { get; set; }
    }
}
