﻿namespace VacancyMicroservice.Api.Models
{
    public class Vacancy
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateOnly CreatedAt { get; set; }
        public bool IsArchived { get; set; }
        public string Position { get; set; }
        public int? SalaryFrom { get; set; }
        public int? SalaryTo { get; set; }
        public string? WorkExperience { get; set; }
        public string EmploymentType { get; set; }
        public bool RemoteWork { get; set; }
        public string VacancyCity { get; set; }
        public string? Address { get; set; }
        public string Description { get; set; }
        public string? EmployerContactPhoneNumber { get; set; }
        public string? EmployerContactEmail { get; set; }
    }
}
