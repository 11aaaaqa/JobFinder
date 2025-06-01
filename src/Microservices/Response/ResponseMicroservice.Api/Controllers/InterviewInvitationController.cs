using Microsoft.AspNetCore.Mvc;
using ResponseMicroservice.Api.Enums;
using ResponseMicroservice.Api.Services.Interview_invitation_services;
using ResponseMicroservice.Api.Services.Pagination;

namespace ResponseMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewInvitationController(IInterviewInvitationService interviewInvitationService,
        ICheckForNextPageExistingService paginationService) : ControllerBase
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
    }
}
