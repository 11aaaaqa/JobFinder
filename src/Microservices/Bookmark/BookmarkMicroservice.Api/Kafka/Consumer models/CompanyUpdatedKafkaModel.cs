namespace BookmarkMicroservice.Api.Kafka.Consumer_models
{
    public class CompanyUpdatedKafkaModel
    {
        public string OldCompanyName { get; set; }
        public string NewCompanyName { get; set; }
    }
}
