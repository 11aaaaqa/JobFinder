﻿using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Resume
{
    public class EditResumeDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        [Required(ErrorMessage = "Поле \"Профессия или должность\" обязательно")]
        public string ResumeTitle { get; set; }
        public List<string>? OccupationTypes { get; set; }
        public List<string>? WorkTypes { get; set; }
        [Required(ErrorMessage = "Поле \"Имя\" обязательно")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Поле \"Фамилия\" обязательно")]
        public string Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? Gender { get; set; }
        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }
        public string? City { get; set; }
        public bool ReadyToMove { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        public string? AboutMe { get; set; }
        public uint? DesiredSalary { get; set; }
        public List<EducationDto>? Educations { get; set; } = new();
        public List<EmployeeExperienceDto>? EmployeeExperience { get; set; } = new();
        public List<ForeignLanguageDto>? ForeignLanguages { get; set; } = new();
    }
}
