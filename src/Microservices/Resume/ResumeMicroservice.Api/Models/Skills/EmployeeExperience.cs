using System.ComponentModel.DataAnnotations;

namespace ResumeMicroservice.Api.Models.Skills
{
    public class EmployeeExperience
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ResumeId { get; set; }
        public string CompanyPost { get; set; }
        public string CompanyName { get; set; }
        public string WorkingFrom { get; set; }
        public string WorkingUntil { get; set; }
        public bool CurrentlyWorkHere { get; set; }
        public TimeSpan WorkingDuration { get; set; }
        public string? Responsibilities { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is EmployeeExperience experience)
                return Id == experience.Id;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
