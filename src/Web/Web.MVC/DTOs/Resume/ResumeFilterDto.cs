namespace Web.MVC.DTOs.Resume
{
    public class ResumeFilterDto
    {
        public string? ResumeTitle { get; set; }
        public List<string>? OccupationTypes { get; set; }
        public List<string>? WorkTypes { get; set; }
        public List<string>? Cities { get; set; }
        public uint? DesiredSalaryTo { get; set; }

        public string? WorkExperience { get; set; }
    }
}
