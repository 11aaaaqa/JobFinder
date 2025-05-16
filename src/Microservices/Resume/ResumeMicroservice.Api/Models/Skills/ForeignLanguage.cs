using System.ComponentModel.DataAnnotations;

namespace ResumeMicroservice.Api.Models.Skills
{
    public class ForeignLanguage
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ResumeId { get; set; }
        public string LanguageName { get; set; }
        public string LanguageProficiencyLevel { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is ForeignLanguage foreignLanguage)
                return Id == foreignLanguage.Id;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
