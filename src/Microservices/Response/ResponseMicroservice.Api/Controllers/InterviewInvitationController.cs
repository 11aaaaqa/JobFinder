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
    public class InterviewInvitationController(IInterviewInvitationService interviewInvitationService,
        ICheckForNextPageExistingService paginationService, IBackgroundJobClient backgroundJob, IVacancyResponseService vacancyResponseService) : ControllerBase
    {
        [HttpGet]
        [Route("GetInterviewInvitationsByCompanyId/{companyId}")]
        public async Task<IActionResult> GetInterviewInvitationsByCompanyIdAsync(Guid companyId, DateTimeOrderByType orderByTimeType, int pageNumber)
            => Ok(await interviewInvitationService.GetInterviewInvitationsByCompanyIdAsync(companyId, orderByTimeType, pageNumber));

        [HttpGet]
        [Route("DoesNextInterviewInvitationsByCompanyIdPageExist/{companyId}")]
        public async Task<IActionResult> DoesNextInterviewInvitationsByCompanyIdPageExistAsync(Guid companyId, DateTimeOrderByType orderByTimeType,
            int currentPageNumber)
            => Ok(await paginationService.DoesNextInterviewInvitationsByCompanyIdPageExistAsync(companyId, orderByTimeType, currentPageNumber));

        [HttpGet]
        [Route("GetInterviewInvitationsByEmployeeId/{employeeId}")]
        public async Task<IActionResult> GetInterviewInvitationsByEmployeeIdAsync(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int pageNumber)
            => Ok(await interviewInvitationService.GetInterviewInvitationsByEmployeeIdAsync(employeeId, searchingQuery, orderByTimeType, pageNumber));

        [HttpGet]
        [Route("DoesNextInterviewInvitationsByEmployeeIdPageExist/{employeeId}")]
        public async Task<IActionResult> DoesNextInterviewInvitationsByEmployeeIdPageExistAsync(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int currentPageNumber) 
            => Ok(await paginationService.DoesNextInterviewInvitationsByEmployeeIdPageExistAsync(employeeId, searchingQuery, orderByTimeType, currentPageNumber));

        [HttpGet]
        [Route("GetCompanyInterviewInvitationsByVacancyId/{vacancyId}")]
        public async Task<IActionResult> GetCompanyInterviewInvitationsByVacancyIdAsync(Guid vacancyId, DateTimeOrderByType orderByTimeType, int pageNumber)
            => Ok(await interviewInvitationService.GetCompanyInterviewInvitationsByVacancyIdAsync(vacancyId, orderByTimeType, pageNumber));

        [HttpGet]
        [Route("DoesNextCompanyInterviewInvitationsByVacancyIdPageExist/{vacancyId}")]
        public async Task<IActionResult> DoesNextCompanyInterviewInvitationsByVacancyIdPageExistAsync(Guid vacancyId, DateTimeOrderByType orderByTimeType,
            int currentPageNumber)
            => Ok(await paginationService.DoesNextCompanyInterviewInvitationsByVacancyIdPageExistAsync(vacancyId, orderByTimeType, currentPageNumber));

        [HttpPost]
        [Route("InviteToInterview")]
        public async Task<IActionResult> AddInterviewInvitationAsync([FromBody]AddInterviewInvitationDto model)
        {
            Guid currentInterviewInvitationId = Guid.NewGuid();

            var jobId =
                backgroundJob.Schedule(() => interviewInvitationService.CloseInterviewAsync(currentInterviewInvitationId),
                    TimeSpan.FromDays(50));

            await interviewInvitationService.AddInvitationAsync(new InterviewInvitation
            {
                Id = currentInterviewInvitationId, InvitationDate = DateTime.UtcNow, EmployeeId = model.EmployeeId,
                VacancyCity = model.VacancyCity, VacancyId = model.VacancyId, EmployeeDateOfBirth = model.EmployeeDateOfBirth,
                VacancyPosition = model.VacancyPosition, EmployeeName = model.EmployeeName, EmployeeSurname = model.EmployeeSurname,
                EmployeeWorkingExperience = model.EmployeeWorkingExperience, VacancySalaryFrom = model.VacancySalaryFrom,
                VacancyWorkExperience = model.VacancyWorkExperience, EmployeeCity = model.EmployeeCity, VacancySalaryTo = model.VacancySalaryTo,
                EmployeeDesiredSalary = model.EmployeeDesiredSalary, VacancyCompanyName = model.VacancyCompanyName, 
                EmployeeResumeId = model.EmployeeResumeId, InvitedCompanyId = model.InvitedCompanyId, IsClosed = false, HangfireDelayedJobId = jobId
            });

            List<VacancyResponse> vacancyResponsesToDelete = 
                await vacancyResponseService.GetWaitingVacancyResponsesAsync(model.EmployeeId, model.InvitedCompanyId);

            await vacancyResponseService.RemoveVacancyResponsesAsync(vacancyResponsesToDelete);

            return Ok();
        }

        [HttpGet]
        [Route("GetInterviewInvitation")]
        public async Task<IActionResult> GetInterviewInvitationAsync(Guid employeeId, Guid companyId)
        {
            var interviewInvitation = await interviewInvitationService.GetInterviewInvitationAsync(employeeId, companyId);
            if (interviewInvitation == null)
                return NotFound();
            return Ok(interviewInvitation);
        }

        [HttpGet]
        [Route("CloseInterview/{interviewInvitationId}")]
        public async Task<IActionResult> CloseInterviewAsync(Guid interviewInvitationId)
        {
            var interviewInvitation = await interviewInvitationService.GetInterviewInvitationByIdAsync(interviewInvitationId);
            if (interviewInvitation == null)
                return NotFound();

            var hangfireDelayedJob = interviewInvitation.HangfireDelayedJobId;
            await interviewInvitationService.CloseInterviewAsync(interviewInvitationId);

            backgroundJob.Delete(hangfireDelayedJob);

            return Ok();
        }

        [HttpGet]
        [Route("GetInterviewInvitationById/{interviewInvitationId}")]
        public async Task<IActionResult> GetInterviewInvitationAsync(Guid interviewInvitationId)
        {
            var interviewInvitation = await interviewInvitationService.GetInterviewInvitationByIdAsync(interviewInvitationId);
            if (interviewInvitation is null)
                return NotFound();
            return Ok(interviewInvitation);
        }
    }
}
