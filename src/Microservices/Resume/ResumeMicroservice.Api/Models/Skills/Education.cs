namespace ResumeMicroservice.Api.Models.Skills
{
    public class Education
    {
        public Guid Id { get; set; }
        public string EducationType { get; set; }
        public string? EducationalInstitution { get; set; }
        public string? Specialization { get; set; }
    }
}
