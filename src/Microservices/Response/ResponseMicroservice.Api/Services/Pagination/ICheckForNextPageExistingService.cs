using GeneralLibrary.Enums;

namespace ResponseMicroservice.Api.Services.Pagination
{
    public interface ICheckForNextPageExistingService
    {
        Task<bool> DoesNextVacancyResponsesByEmployeeIdPageExistAsync(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int currentPageNumber);
        Task<bool> DoesNextVacancyResponsesByCompanyIdPageExistAsync(Guid companyId, DateTimeOrderByType orderByTimeType, int currentPageNumber);
        Task<bool> DoesNextCompanyVacancyResponsesByVacancyIdPageExistAsync(Guid vacancyId, DateTimeOrderByType orderByTimeType, int currentPageNumber);
        Task<bool> DoesNextInterviewInvitationsByCompanyIdPageExistAsync(Guid companyId, bool closedInterviews, DateTimeOrderByType orderByTimeType, int currentPageNumber);
        Task<bool> DoesNextInterviewInvitationsByEmployeeIdPageExistAsync(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int currentPageNumber);
        Task<bool> DoesNextCompanyInterviewInvitationsByVacancyIdPageExistAsync(Guid vacancyId, bool closedInterviews, DateTimeOrderByType orderByTimeType, int currentPageNumber);
    }
}
