using System.ComponentModel.DataAnnotations;

namespace ResumeMicroservice.Api.Models.Skills
{
    public class Education
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ResumeId { get; set; }
        public string EducationType { get; set; }
        public string? EducationalInstitution { get; set; }
        public string? Specialization { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is Education education)
                return Id == education.Id;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
