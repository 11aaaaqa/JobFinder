namespace CompanyMicroservice.Api.Kafka.Consumer_models
{
    public class CompanyRatingUpdatedConsumerModel
    {
        public Guid CompanyId { get; set; }
        public double NewCompanyRating { get; set; }
    }
}
