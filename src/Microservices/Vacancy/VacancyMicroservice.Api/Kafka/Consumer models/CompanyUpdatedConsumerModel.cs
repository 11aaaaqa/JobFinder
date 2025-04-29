namespace VacancyMicroservice.Api.Kafka.Consumer_models
{
    public class CompanyUpdatedConsumerModel
    {
        public Guid CompanyId { get; set; }
        public string NewCompanyName { get; set; }
    }
}
