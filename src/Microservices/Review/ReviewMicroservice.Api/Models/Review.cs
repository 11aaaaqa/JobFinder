namespace ReviewMicroservice.Api.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Position { get; set; }
        public string City { get; set; }
        public string WorkingState { get; set; }
        public string WorkingTime { get; set; }
        public string Advantages { get; set; }
        public string CanBeImproved { get; set; }
        public byte WorkingConditions { get; set; }
        public byte Colleagues { get; set; }
        public byte Management { get; set; }
        public byte GrowthOpportunities { get; set; }
        public byte RestConditions { get; set; }
        public byte SalaryLevel { get; set; }
        public byte GeneralEstimation { get; set; }
        public DateOnly CreatedAt { get; set; }
    }
}
