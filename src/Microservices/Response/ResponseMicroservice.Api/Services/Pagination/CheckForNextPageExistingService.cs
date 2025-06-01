using Microsoft.EntityFrameworkCore;
using ResponseMicroservice.Api.Constants;
using ResponseMicroservice.Api.Database;
using ResponseMicroservice.Api.Enums;

namespace ResponseMicroservice.Api.Services.Pagination
{
    public class CheckForNextPageExistingService(ApplicationDbContext context) : ICheckForNextPageExistingService
    {
        public async Task<bool> DoesNextVacancyResponsesByEmployeeIdPageExistAsync(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int currentPageNumber)
        {
            var vacancyResponses = context.VacancyResponses.Where(x => x.EmployeeId == employeeId).AsQueryable();
            if (searchingQuery is not null)
                vacancyResponses = vacancyResponses.Where(x => x.VacancyPosition.ToLower().Contains(searchingQuery.ToLower()));

            switch (orderByTimeType)
            {
                case DateTimeOrderByType.Ascending:
                    vacancyResponses = vacancyResponses.OrderBy(x => x.ResponseDate);
                    break;
                case DateTimeOrderByType.Descending:
                    vacancyResponses = vacancyResponses.OrderByDescending(x => x.ResponseDate);
                    break;
            }

            return await vacancyResponses.Skip(currentPageNumber * PaginationConstants.VacancyResponsePageSize).CountAsync() > 0;
        }

        public async Task<bool> DoesNextVacancyResponsesByCompanyIdPageExistAsync(Guid companyId, DateTimeOrderByType orderByTimeType,
            int currentPageNumber)
        {
            var vacancyResponses = context.VacancyResponses.Where(x => x.VacancyCompanyId == companyId).AsQueryable();

            switch (orderByTimeType)
            {
                case DateTimeOrderByType.Ascending:
                    vacancyResponses = vacancyResponses.OrderBy(x => x.ResponseDate);
                    break;
                case DateTimeOrderByType.Descending:
                    vacancyResponses = vacancyResponses.OrderByDescending(x => x.ResponseDate);
                    break;
            }

            return await vacancyResponses.Skip(currentPageNumber * PaginationConstants.VacancyResponsePageSize).CountAsync() > 0;
        }

        public async Task<bool> DoesNextCompanyVacancyResponsesByVacancyIdPageExistAsync(Guid vacancyId, DateTimeOrderByType orderByTimeType,
            int currentPageNumber)
        {
            var vacancyResponses = context.VacancyResponses.Where(x => x.VacancyId == vacancyId).AsQueryable();

            switch (orderByTimeType)
            {
                case DateTimeOrderByType.Ascending:
                    vacancyResponses = vacancyResponses.OrderBy(x => x.ResponseDate);
                    break;
                case DateTimeOrderByType.Descending:
                    vacancyResponses = vacancyResponses.OrderByDescending(x => x.ResponseDate);
                    break;
            }

            return await vacancyResponses.Skip(currentPageNumber * PaginationConstants.VacancyResponsePageSize).CountAsync() > 0;
        }

        public async Task<bool> DoesNextInterviewInvitationsByCompanyIdPageExistAsync(Guid companyId, DateTimeOrderByType orderByTimeType,
            int currentPageNumber)
        {
            var interviewInvitations =
                context.InterviewInvitations.Where(x => x.InvitedCompanyId == companyId).AsQueryable();

            switch (orderByTimeType)
            {
                case DateTimeOrderByType.Ascending:
                    interviewInvitations = interviewInvitations.OrderBy(x => x.InvitationDate);
                    break;
                case DateTimeOrderByType.Descending:
                    interviewInvitations = interviewInvitations.OrderByDescending(x => x.InvitationDate);
                    break;
            }

            return await interviewInvitations.Skip(currentPageNumber * PaginationConstants.InterviewInvitationPageSize).CountAsync() > 0;
        }

        public async Task<bool> DoesNextInterviewInvitationsByEmployeeIdPageExistAsync(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int currentPageNumber)
        {
            var interviewInvitations =
                context.InterviewInvitations.Where(x => x.EmployeeId == employeeId).AsQueryable();

            if (searchingQuery is not null)
                interviewInvitations = interviewInvitations.Where(x => x.VacancyPosition.ToLower().Contains(searchingQuery.ToLower()));

            switch (orderByTimeType)
            {
                case DateTimeOrderByType.Ascending:
                    interviewInvitations = interviewInvitations.OrderBy(x => x.InvitationDate);
                    break;
                case DateTimeOrderByType.Descending:
                    interviewInvitations = interviewInvitations.OrderByDescending(x => x.InvitationDate);
                    break;
            }

            return await interviewInvitations.Skip(currentPageNumber * PaginationConstants.InterviewInvitationPageSize).CountAsync() > 0;
        }

        public async Task<bool> DoesNextCompanyInterviewInvitationsByVacancyIdPageExistAsync(Guid vacancyId, DateTimeOrderByType orderByTimeType,
            int currentPageNumber)
        {
            var interviewInvitations =
                context.InterviewInvitations.Where(x => x.VacancyId == vacancyId).AsQueryable();

            switch (orderByTimeType)
            {
                case DateTimeOrderByType.Ascending:
                    interviewInvitations = interviewInvitations.OrderBy(x => x.InvitationDate);
                    break;
                case DateTimeOrderByType.Descending:
                    interviewInvitations = interviewInvitations.OrderByDescending(x => x.InvitationDate);
                    break;
            }

            return await interviewInvitations.Skip(currentPageNumber * PaginationConstants.InterviewInvitationPageSize).CountAsync() > 0;
        }
    }
}
