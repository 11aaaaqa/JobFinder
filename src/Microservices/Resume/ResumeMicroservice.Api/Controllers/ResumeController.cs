using System.Text.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using ResumeMicroservice.Api.DTOs;
using ResumeMicroservice.Api.Kafka.Producing;
using ResumeMicroservice.Api.Models;
using ResumeMicroservice.Api.Services.Pagination;
using ResumeMicroservice.Api.Services.Repositories;

namespace ResumeMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController(IResumeRepository resumeRepository, 
        ICheckForNextPageExistingService checkForNextPageService, IKafkaProducer kafkaProducer) : ControllerBase
    {
        [HttpGet]
        [Route("GetResumeById/{resumeId}")]
        public async Task<IActionResult> GetResumeByIdAsync(Guid resumeId)
        {
            var resume = await resumeRepository.GetResumeByIdAsync(resumeId);
            if (resume == null)
                return BadRequest();

            return Ok(resume);
        }
        
        [HttpGet]
        [Route("GetAllResumes")]
        public async Task<IActionResult> GetAllResumesAsync(string? searchingQuery, int pageNumber)
            => Ok(await resumeRepository.GetAllResumesAsync(searchingQuery, pageNumber));

        [HttpGet]
        [Route("DoesNextAllResumesPageExist")]
        public async Task<IActionResult> DoesNextAllResumesPageExistAsync(string? searchingQuery, int currentPageNumber)
            => Ok(await checkForNextPageService.DoesNextAllResumesPageExistAsync(searchingQuery, currentPageNumber));

        [HttpGet]
        [Route("GetResumesWithActiveStatus")]
        public async Task<IActionResult> GetResumesWithActiveStatusAsync(string? searchingQuery, int pageNumber)
            => Ok(await resumeRepository.GetResumesWithActiveStatusAsync(searchingQuery, pageNumber));

        [HttpGet]
        [Route("DoesNextResumesWithActiveStatusPageExist")]
        public async Task<IActionResult> DoesNextResumesWithActiveStatusPageExistAsync(string? searchingQuery, int currentPageNumber)
            => Ok(await checkForNextPageService.DoesNextResumesWithActiveStatusPageExistAsync(searchingQuery, currentPageNumber));

        [HttpPost]
        [Route("GetFilteredResumes")]
        public async Task<IActionResult> GetFilteredResumesAsync([FromBody] ResumeFilterModel model, string? searchingQuery, int pageNumber)
            => Ok(await resumeRepository.GetFilteredResumesAsync(model, searchingQuery, pageNumber));

        [HttpPost]
        [Route("DoesNextFilteredResumesPageExist")]
        public async Task<IActionResult> DoesNextFilteredResumesPageExistAsync([FromBody] ResumeFilterModel model,
            string? searchingQuery, int currentPageNumber)
            => Ok(await checkForNextPageService.DoesNextFilteredResumesPageExistAsync(model, searchingQuery, currentPageNumber));

        [HttpGet]
        [Route("GetResumesByEmployeeId/{employeeId}")]
        public async Task<IActionResult> GetResumesByEmployeeIdAsync(Guid employeeId)
            => Ok(await resumeRepository.GetResumesByEmployeeIdAsync(employeeId));

        [HttpDelete]
        [Route("DeleteResume/{resumeId}")]
        public async Task<IActionResult> DeleteResumeAsync(Guid resumeId)
        {
            await resumeRepository.DeleteResumeAsync(resumeId);
            await kafkaProducer.ProduceAsync("resume-deleted-topic", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(new {resumeId})
            });
            return Ok();
        }

        [HttpPost]
        [Route("AddResume")]
        public async Task<IActionResult> AddResumeAsync([FromBody] AddResumeDto model)
        {
            TimeSpan workingExperience = TimeSpan.Zero;
            if (model.EmployeeExperience is not null)
            {
                foreach (var experience in model.EmployeeExperience)
                {
                    workingExperience = workingExperience.Add(experience.WorkingDuration);
                }
            }

            await resumeRepository.AddResumeAsync(new Resume
            {
                Name = model.Name, AboutMe = model.AboutMe, City = model.City, DateOfBirth = model.DateOfBirth,
                DesiredSalary = model.DesiredSalary, Educations = model.Educations, Email = model.Email, EmployeeExperience = model.EmployeeExperience,
                EmployeeId = model.EmployeeId, ForeignLanguages = model.ForeignLanguages, Gender = model.Gender,
                Id = model.Id, Surname = model.Surname, OccupationTypes = model.OccupationTypes, Status = model.Status,
                PhoneNumber = model.PhoneNumber, Patronymic = model.Patronymic, ReadyToMove = model.ReadyToMove, ResumeTitle = model.ResumeTitle,
                WorkTypes = model.WorkTypes, WorkingExperience = workingExperience
            });
            return Ok();
        }

        [HttpPut]
        [Route("UpdateResume")]
        public async Task<IActionResult> UpdateResumeAsync([FromBody] UpdateResumeControllerDto model)
        {
            TimeSpan workingExperience = TimeSpan.Zero;
            if (model.EmployeeExperience is not null)
            {
                foreach (var experience in model.EmployeeExperience)
                {
                    workingExperience = workingExperience.Add(experience.WorkingDuration);
                }
            }

            await resumeRepository.UpdateResumeAsync(new UpdateResumeDto
            {
                Name = model.Name, AboutMe = model.AboutMe, City = model.City, DateOfBirth = model.DateOfBirth, DesiredSalary = model.DesiredSalary,
                Educations = model.Educations, Email = model.Email, EmployeeExperience = model.EmployeeExperience,
                ForeignLanguages = model.ForeignLanguages, Gender = model.Gender, Id = model.Id, Surname = model.Surname,
                PhoneNumber = model.PhoneNumber, OccupationTypes = model.OccupationTypes, Patronymic = model.Patronymic,
                ReadyToMove = model.ReadyToMove, ResumeTitle = model.ResumeTitle, WorkTypes = model.WorkTypes, WorkingExperience = workingExperience
            });

            return Ok();
        }
    }
}
