﻿using GeneralLibrary.Enums;
using ResponseMicroservice.Api.Models;

namespace ResponseMicroservice.Api.Services.Interview_invitation_services
{
    public interface IInterviewInvitationService
    {
        Task<InterviewInvitation?> GetInterviewInvitationByIdAsync(Guid interviewInvitationId);
        Task<List<InterviewInvitation>> GetInterviewInvitationsByCompanyIdAsync(Guid companyId, string? searchingQuery, DateTimeOrderByType orderByTimeType,
            int pageNumber);
        Task<List<InterviewInvitation>> GetInterviewInvitationsByEmployeeIdAsync(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int pageNumber);
        Task<List<InterviewInvitation>> GetCompanyInterviewInvitationsByVacancyIdAsync(Guid vacancyId, string? searchingQuery, DateTimeOrderByType orderByTimeType,
            int pageNumber);
        Task AddInvitationAsync(InterviewInvitation model);
        Task<InterviewInvitation?> GetInterviewInvitationAsync(Guid employeeId, Guid companyId);
        Task CloseInterviewAsync(Guid interviewInvitationId);
    }
}
