namespace Web.MVC.Models.ApiResponses.Review
{
    public class ReviewResponse
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
        public int WorkingConditions { get; set; }
        public int Colleagues { get; set; }
        public int Management { get; set; }
        public int GrowthOpportunities { get; set; }
        public int RestConditions { get; set; }
        public int SalaryLevel { get; set; }
        public double GeneralEstimation { get; set; }
        public DateOnly CreatedAt { get; set; }
    }
}
