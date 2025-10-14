using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Review
{
    public class AddReviewDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Position { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Required]
        public string? WorkingState { get; set; }

        [Required]
        public string? WorkingTime { get; set; }

        [Required]
        [StringLength(500)]
        public string Advantages { get; set; }

        [Required]
        [StringLength(500)]
        public string CanBeImproved { get; set; }

        [Required]
        public int WorkingConditions { get; set; }

        [Required]
        public int Colleagues { get; set; }

        [Required]
        public int Management { get; set; }

        [Required]
        public int GrowthOpportunities { get; set; }

        [Required]
        public int RestConditions { get; set; }

        [Required]
        public int SalaryLevel { get; set; }
    }
}
