namespace EmployeeMicroservice.Api.Models.EmployeeSkills
{
    public class Education
    {
        public Guid Id { get; set; }
        public string EducationType { get; set; }
        public string? EducationForm { get; set; }
        public string? EducationalInstitution { get; set; }
        public string? Specialization { get; set; }
    }
}
