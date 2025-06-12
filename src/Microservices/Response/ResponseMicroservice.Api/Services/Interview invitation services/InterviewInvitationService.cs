using GeneralLibrary.Enums;
using Microsoft.EntityFrameworkCore;
using ResponseMicroservice.Api.Constants;
using ResponseMicroservice.Api.Database;
using ResponseMicroservice.Api.Models;

namespace ResponseMicroservice.Api.Services.Interview_invitation_services
{
    public class InterviewInvitationService(ApplicationDbContext context) : IInterviewInvitationService
    {
        public async Task<InterviewInvitation?> GetInterviewInvitationByIdAsync(Guid interviewInvitationId)
            => await context.InterviewInvitations.SingleOrDefaultAsync(x => x.Id == interviewInvitationId);

        public async Task<List<InterviewInvitation>> GetInterviewInvitationsByCompanyIdAsync(Guid companyId, DateTimeOrderByType orderByTimeType, int pageNumber)
        {
            var interviewInvitations = context.InterviewInvitations
                .Where(x => x.InvitedCompanyId == companyId)
                .Where(x => x.IsClosed == false)
                .AsQueryable();

            switch (orderByTimeType)
            {
                case DateTimeOrderByType.Ascending:
                    interviewInvitations = interviewInvitations.OrderBy(x => x.InvitationDate);
                    break;
                case DateTimeOrderByType.Descending:
                    interviewInvitations = interviewInvitations.OrderByDescending(x => x.InvitationDate);
                    break;
            }

            return await interviewInvitations.Skip((pageNumber - 1) * PaginationConstants.InterviewInvitationPageSize)
                .Take(PaginationConstants.InterviewInvitationPageSize).ToListAsync();
        }

        public async Task<List<InterviewInvitation>> GetInterviewInvitationsByEmployeeIdAsync(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int pageNumber)
        {
            var interviewInvitations = context.InterviewInvitations
                .Where(x => x.EmployeeId == employeeId)
                .Where(x => x.IsClosed == false)
                .AsQueryable();

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

            return await interviewInvitations.Skip((pageNumber - 1) * PaginationConstants.InterviewInvitationPageSize)
                .Take(PaginationConstants.InterviewInvitationPageSize).ToListAsync();
        }

        public async Task<List<InterviewInvitation>> GetCompanyInterviewInvitationsByVacancyIdAsync(Guid vacancyId,  DateTimeOrderByType orderByTimeType,
            int pageNumber)
        {
            var interviewInvitations = context.InterviewInvitations
                .Where(x => x.VacancyId == vacancyId)
                .Where(x => x.IsClosed == false)
                .AsQueryable();

            switch (orderByTimeType)
            {
                case DateTimeOrderByType.Ascending:
                    interviewInvitations = interviewInvitations.OrderBy(x => x.InvitationDate);
                    break;
                case DateTimeOrderByType.Descending:
                    interviewInvitations = interviewInvitations.OrderByDescending(x => x.InvitationDate);
                    break;
            }

            return await interviewInvitations.Skip((pageNumber - 1) * PaginationConstants.InterviewInvitationPageSize)
                .Take(PaginationConstants.InterviewInvitationPageSize).ToListAsync();
        }

        public async Task AddInvitationAsync(InterviewInvitation model)
        {
            await context.InterviewInvitations.AddAsync(model);
            await context.SaveChangesAsync();
        }

        public async Task<bool> HasEmployeeInvitedToInterviewAsync(Guid employeeId, Guid vacancyId)
        {
            var interviewInvitations = await context.InterviewInvitations.Where(x => x.EmployeeId == employeeId)
                .Where(x => x.VacancyId == vacancyId).ToListAsync();

            foreach (var interviewInvitation in interviewInvitations)
            {
                if (!interviewInvitation.IsClosed)
                    return true;
            }

            return false;
        }

        public async Task CloseInterviewAsync(Guid interviewInvitationId)
        {
            var interviewInvitation = await context.InterviewInvitations.SingleOrDefaultAsync(x => x.Id == interviewInvitationId);

            if (interviewInvitation is null || interviewInvitation.IsClosed)
                return;

            interviewInvitation.IsClosed = true;
            await context.SaveChangesAsync();
        }
    }
}
