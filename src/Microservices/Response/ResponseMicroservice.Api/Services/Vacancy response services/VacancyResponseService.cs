using GeneralLibrary.Constants;
using GeneralLibrary.Enums;
using Microsoft.EntityFrameworkCore;
using ResponseMicroservice.Api.Constants;
using ResponseMicroservice.Api.Database;
using ResponseMicroservice.Api.Models;

namespace ResponseMicroservice.Api.Services.Vacancy_response_services
{
    public class VacancyResponseService(ApplicationDbContext context) : IVacancyResponseService
    {
        public async Task<VacancyResponse?> GetVacancyResponseByIdAsync(Guid vacancyResponseId)
            => await context.VacancyResponses.SingleOrDefaultAsync(x => x.Id == vacancyResponseId);

        public async Task<List<VacancyResponse>> GetVacancyResponsesByEmployeeIdAsync(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int pageNumber)
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

            return await vacancyResponses.Skip((pageNumber - 1) * PaginationConstants.VacancyResponsePageSize)
                .Take(PaginationConstants.VacancyResponsePageSize)
                .ToListAsync();
        }

        public async Task<List<VacancyResponse>> GetVacancyResponsesByCompanyIdAsync(Guid companyId, DateTimeOrderByType orderByTimeType, int pageNumber)
        {
            var vacancyResponses = context.VacancyResponses.Where(x => x.VacancyCompanyId == companyId)
                .Where(x => x.ResponseStatus == VacancyResponseStatusConstants.Waiting).AsQueryable();

            switch (orderByTimeType)
            {
                case DateTimeOrderByType.Ascending:
                    vacancyResponses = vacancyResponses.OrderBy(x => x.ResponseDate);
                    break;
                case DateTimeOrderByType.Descending:
                    vacancyResponses = vacancyResponses.OrderByDescending(x => x.ResponseDate);
                    break;
            }

            return await vacancyResponses.Skip((pageNumber - 1) * PaginationConstants.VacancyResponsePageSize)
                .Take(PaginationConstants.VacancyResponsePageSize)
                .ToListAsync();
        }

        public async Task<List<VacancyResponse>> GetCompanyVacancyResponsesByVacancyIdAsync(Guid vacancyId, DateTimeOrderByType orderByTimeType, int pageNumber)
        {
            var vacancyResponses = context.VacancyResponses.Where(x => x.VacancyId == vacancyId)
                .Where(x => x.ResponseStatus == VacancyResponseStatusConstants.Waiting).AsQueryable();

            switch (orderByTimeType)
            {
                case DateTimeOrderByType.Ascending:
                    vacancyResponses = vacancyResponses.OrderBy(x => x.ResponseDate);
                    break;
                case DateTimeOrderByType.Descending:
                    vacancyResponses = vacancyResponses.OrderByDescending(x => x.ResponseDate);
                    break;
            }

            return await vacancyResponses.Skip((pageNumber - 1) * PaginationConstants.VacancyResponsePageSize)
                .Take(PaginationConstants.VacancyResponsePageSize)
                .ToListAsync();
        }

        public async Task SetVacancyResponseStatusAsync(Guid vacancyResponseId, string status)
        {
            var vacancyResponse = await context.VacancyResponses.SingleOrDefaultAsync(x => x.Id == vacancyResponseId);

            if(vacancyResponse is null)
                return;

            vacancyResponse.ResponseStatus = status;
            await context.SaveChangesAsync();
        }

        public async Task AddVacancyResponseAsync(VacancyResponse model)
        {
            var vacancyResponses = await context.VacancyResponses.Where(x => x.EmployeeId == model.EmployeeId)
                .Where(x => x.VacancyId == model.VacancyId).ToListAsync();
            if(vacancyResponses.Count > 0) 
                context.VacancyResponses.RemoveRange(vacancyResponses);

            await context.VacancyResponses.AddAsync(model);
            await context.SaveChangesAsync();
        }

        public async Task<bool> HasEmployeeRespondedToVacancyAsync(Guid employeeId, Guid vacancyId)
        {
            var vacancyResponse = await context.VacancyResponses.Where(x => x.VacancyId == vacancyId)
                .SingleOrDefaultAsync(x => x.EmployeeId == employeeId);

            if (vacancyResponse is null)
                return false;

            if (vacancyResponse.ResponseDate.AddMonths(1) > DateTime.UtcNow)
                return true;

            return false;
        }

        public async Task<List<VacancyResponse>> GetWaitingVacancyResponsesAsync(Guid employeeId, Guid companyId)
        {
            var vacancyResponses = await context.VacancyResponses
                .Where(x => x.EmployeeId == employeeId)
                .Where(x => x.VacancyCompanyId == companyId)
                .Where(x => x.ResponseStatus == VacancyResponseStatusConstants.Waiting)
                .ToListAsync();
            return vacancyResponses;
        }

        public async Task RemoveVacancyResponsesAsync(List<VacancyResponse> vacancyResponses)
        {
            context.VacancyResponses.RemoveRange(vacancyResponses);
            await context.SaveChangesAsync();
        }
    }
}
