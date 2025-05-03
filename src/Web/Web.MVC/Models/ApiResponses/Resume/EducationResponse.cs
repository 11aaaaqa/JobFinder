namespace Web.MVC.Models.ApiResponses.Resume
{
    public class EducationResponse
    {
        public Guid Id { get; set; }
        public string EducationType { get; set; }
        public string? EducationalInstitution { get; set; }
        public string? Specialization { get; set; }
    }
}
