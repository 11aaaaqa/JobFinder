namespace BookmarkMicroservice.Api.Kafka.Consumer_models
{
    public class VacancyUpdatedKafkaModel
    {
        public Guid VacancyId { get; set; }
        public string NewPosition { get;set; }
        public int? NewSalaryFrom { get; set; }
        public int? NewSalaryTo { get; set; }
        public string? NewWorkExperience { get; set; }
        public string NewVacancyCity { get; set; }
    }
}
