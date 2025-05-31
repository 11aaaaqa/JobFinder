using ResponseMicroservice.Api.Enums;
using ResponseMicroservice.Api.Models;

namespace ResponseMicroservice.Api.Services.Interview_invitation_services
{
    public interface IInterviewInvitationService
    {
        Task<List<InterviewInvitation>> GetInterviewInvitationsByCompanyIdAsync(Guid companyId, DateTimeOrderByType orderByTimeType, int pageNumber);
        Task<List<InterviewInvitation>> GetInterviewInvitationsByEmployeeIdAsync(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int pageNumber);
        Task<List<InterviewInvitation>> GetCompanyInterviewInvitationsByVacancyIdAsync(Guid vacancyId, DateTimeOrderByType orderByTimeType, int pageNumber);
        Task AddInvitationAsync(InterviewInvitation model);
    }
}
