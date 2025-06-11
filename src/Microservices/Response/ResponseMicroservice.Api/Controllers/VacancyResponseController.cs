using GeneralLibrary.Constants;
using GeneralLibrary.Enums;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using ResponseMicroservice.Api.DTOs;
using ResponseMicroservice.Api.Models;
using ResponseMicroservice.Api.Services.Interview_invitation_services;
using ResponseMicroservice.Api.Services.Pagination;
using ResponseMicroservice.Api.Services.Vacancy_response_services;

namespace ResponseMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacancyResponseController(IVacancyResponseService vacancyResponseService,
        ICheckForNextPageExistingService paginationService, IInterviewInvitationService interviewInvitationService,
        IBackgroundJobClient backgroundJob) : ControllerBase
    {
        [HttpGet]
        [Route("GetVacancyResponseById/{vacancyResponseId}")]
        public async Task<IActionResult> GetVacancyResponseById(Guid vacancyResponseId)
        {
            var vacancyResponse = await vacancyResponseService.GetVacancyResponseByIdAsync(vacancyResponseId);
            if (vacancyResponse is null)
                return NotFound();

            return Ok(vacancyResponse);
        }

        [HttpGet]
        [Route("GetVacancyResponsesByEmployeeId/{employeeId}")]
        public async Task<IActionResult> GetVacancyResponsesByEmployeeIdAsync(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int pageNumber)
            => Ok(await vacancyResponseService.GetVacancyResponsesByEmployeeIdAsync(employeeId, searchingQuery, orderByTimeType, pageNumber));

        [HttpGet]
        [Route("DoesNextVacancyResponsesByEmployeeIdPageExist/{employeeId}")]
        public async Task<IActionResult> DoesNextVacancyResponsesByEmployeeIdPageExist(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int currentPageNumber)
            => Ok(await paginationService.DoesNextVacancyResponsesByEmployeeIdPageExistAsync(employeeId, searchingQuery, orderByTimeType, currentPageNumber));

        [HttpGet]
        [Route("GetVacancyResponsesByCompanyId/{companyId}")]
        public async Task<IActionResult> GetVacancyResponsesByCompanyIdAsync(Guid companyId, DateTimeOrderByType orderByTimeType, int pageNumber)
            => Ok(await vacancyResponseService.GetVacancyResponsesByCompanyIdAsync(companyId, orderByTimeType, pageNumber));

        [HttpGet]
        [Route("DoesNextVacancyResponsesByCompanyIdPageExist/{companyId}")]
        public async Task<IActionResult> DoesNextVacancyResponsesByCompanyIdPageExistAsync(Guid companyId, DateTimeOrderByType orderByTimeType,
            int currentPageNumber)
            => Ok(await paginationService.DoesNextVacancyResponsesByCompanyIdPageExistAsync(companyId, orderByTimeType, currentPageNumber));

        [HttpGet]
        [Route("GetCompanyVacancyResponsesByVacancyId/{vacancyId}")]
        public async Task<IActionResult> GetCompanyVacancyResponsesByVacancyIdAsync(Guid vacancyId, DateTimeOrderByType orderByTimeType, int pageNumber)
            => Ok(await vacancyResponseService.GetCompanyVacancyResponsesByVacancyIdAsync(vacancyId, orderByTimeType, pageNumber));

        [HttpGet]
        [Route("DoesNextCompanyVacancyResponsesByVacancyIdPageExist/{vacancyId}")]
        public async Task<IActionResult> DoesNextCompanyVacancyResponsesByVacancyIdPageExistAsync(Guid vacancyId,
            DateTimeOrderByType orderByTimeType, int currentPageNumber)
            => Ok(await paginationService.DoesNextCompanyVacancyResponsesByVacancyIdPageExistAsync(vacancyId, orderByTimeType, currentPageNumber));

        [HttpPost]
        [Route("AddVacancyResponse")]
        public async Task<IActionResult> AddVacancyResponseAsync([FromBody] AddVacancyResponseDto model)
        {
            await vacancyResponseService.AddVacancyResponseAsync(new VacancyResponse
            {
                EmployeeCity = model.EmployeeCity, EmployeeDateOfBirth = model.EmployeeDateOfBirth, EmployeeId = model.EmployeeId,
                EmployeeName = model.EmployeeName, EmployeeSurname = model.EmployeeSurname, EmployeeWorkingExperience = model.EmployeeWorkingExperience,
                Id = Guid.NewGuid(), VacancyCity = model.VacancyCity, VacancyId = model.VacancyId, ResponseDate = DateTime.UtcNow,
                ResponseStatus = VacancyResponseStatusConstants.Waiting, RespondedEmployeeResumeId = model.RespondedEmployeeResumeId,
                VacancyCompanyId = model.VacancyCompanyId, VacancyPosition = model.VacancyPosition, VacancySalaryFrom = model.VacancySalaryFrom,
                VacancySalaryTo = model.VacancySalaryTo, VacancyWorkExperience = model.VacancyWorkExperience, VacancyCompanyName = model.VacancyCompanyName,
                EmployeeDesiredSalary = model.EmployeeDesiredSalary
            });
            return Ok();
        }

        [HttpGet]
        [Route("HasEmployeeRespondedToVacancy")]
        public async Task<IActionResult> HasEmployeeRespondedToVacancyAsync(Guid employeeId, Guid vacancyId)
            => Ok(await vacancyResponseService.HasEmployeeRespondedToVacancyAsync(employeeId, vacancyId));

        [HttpGet]
        [Route("RejectVacancyResponse/{vacancyResponseId}")]
        public async Task<IActionResult> RejectVacancyResponseAsync(Guid vacancyResponseId)
        {
            await vacancyResponseService.SetVacancyResponseStatusAsync(vacancyResponseId, VacancyResponseStatusConstants.Rejected);
            return Ok();
        }

        [HttpGet]
        [Route("AcceptVacancyResponse/{vacancyResponseId}")]
        public async Task<IActionResult> AcceptVacancyResponseAsync(Guid vacancyResponseId)
        {
            var vacancyResponse = await vacancyResponseService.GetVacancyResponseByIdAsync(vacancyResponseId);
            if (vacancyResponse is null)
                return BadRequest();

            Guid currentInterviewInvitationId = Guid.NewGuid();
            await interviewInvitationService.AddInvitationAsync(new InterviewInvitation
            {
                EmployeeCity = vacancyResponse.EmployeeCity, EmployeeDateOfBirth = vacancyResponse.EmployeeDateOfBirth,
                EmployeeId = vacancyResponse.EmployeeId, EmployeeName = vacancyResponse.EmployeeName,
                EmployeeResumeId = vacancyResponse.RespondedEmployeeResumeId, EmployeeSurname = vacancyResponse.EmployeeSurname,
                EmployeeWorkingExperience = vacancyResponse.EmployeeWorkingExperience, Id = currentInterviewInvitationId,
                VacancyCity = vacancyResponse.VacancyCity, VacancyId = vacancyResponse.VacancyId,
                InvitationDate = DateTime.UtcNow, InvitedCompanyId = vacancyResponse.VacancyCompanyId,
                VacancyPosition = vacancyResponse.VacancyPosition, VacancySalaryFrom = vacancyResponse.VacancySalaryFrom,
                VacancySalaryTo = vacancyResponse.VacancySalaryTo, VacancyWorkExperience = vacancyResponse.VacancyWorkExperience,
                VacancyCompanyName = vacancyResponse.VacancyCompanyName, EmployeeDesiredSalary = vacancyResponse.EmployeeDesiredSalary,
                IsClosed = false
            });
            await vacancyResponseService.SetVacancyResponseStatusAsync(vacancyResponseId, VacancyResponseStatusConstants.Accepted);

            backgroundJob.Schedule(() => interviewInvitationService.CloseInterviewAsync(currentInterviewInvitationId), TimeSpan.FromDays(50));

            return Ok();
        }
    }
}
