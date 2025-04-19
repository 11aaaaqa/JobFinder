using Microsoft.EntityFrameworkCore;
using VacancyMicroservice.Api.Constants;
using VacancyMicroservice.Api.Database;
using VacancyMicroservice.Api.DTOs;
using VacancyMicroservice.Api.Models;

namespace VacancyMicroservice.Api.Services.Pagination
{
    public class PaginationService(ApplicationDbContext context) : IPaginationService
    {
        public async Task<bool> DoesNextAllVacanciesPageExist(int currentPageNumber)
        {
            var remainedVacanciesCount = await context.Vacancies.Skip(currentPageNumber * PaginationConstants.VacancyPageSize).CountAsync();
            return remainedVacanciesCount > 0;
        }

        public async Task<bool> DoesNextSearchVacanciesPageExist(string searchingQuery, int currentPageNumber)
        {
            searchingQuery = searchingQuery.ToLower();
            var remainedVacanciesCount = await context.Vacancies.Where(x =>
                    x.CompanyName.ToLower().Contains(searchingQuery) |
                    x.Position.ToLower().Contains(searchingQuery))
                .Skip(currentPageNumber * PaginationConstants.VacancyPageSize).CountAsync();
            return remainedVacanciesCount > 0;
        }

        public async Task<bool> DoesNextFilteredVacanciesPageExist(GetFilteredVacanciesDto model, int currentPageNumber)
        {
            var vacancies = context.Vacancies.AsQueryable();
            if (model.Position is not null)
                vacancies = vacancies.Where(x => x.Position.ToLower() == model.Position.ToLower());
            if (model.SalaryFrom is not null)
                vacancies = vacancies.Where(x => x.SalaryTo >= model.SalaryFrom);
            if (model.WorkExperience is not null)
                vacancies = vacancies.Where(x => x.WorkExperience.ToLower() == model.WorkExperience.ToLower());
            if (model.EmploymentType is not null)
                vacancies = vacancies.Where(x => x.EmploymentType.ToLower() == model.EmploymentType.ToLower());
            if (model.RemoteWork is not null)
                vacancies = vacancies.Where(x => x.RemoteWork == model.RemoteWork);
            if (model.VacancyCities is not null)
                vacancies = vacancies.Where(x => model.VacancyCities.Contains(x.VacancyCity));

            return await vacancies.Skip(currentPageNumber * PaginationConstants.VacancyPageSize).CountAsync() > 0;
        }

        public async Task<bool> DoesNextSearchFilteredVacanciesPageExist(GetFilteredVacanciesDto model, string searchingQuery,
            int currentPageNumber)
        {
            var vacancies = context.Vacancies.AsQueryable();
            if (model.Position is not null)
                vacancies = vacancies.Where(x => x.Position.ToLower() == model.Position.ToLower());
            if (model.SalaryFrom is not null)
                vacancies = vacancies.Where(x => x.SalaryTo >= model.SalaryFrom);
            if (model.WorkExperience is not null)
                vacancies = vacancies.Where(x => x.WorkExperience.ToLower() == model.WorkExperience.ToLower());
            if (model.EmploymentType is not null)
                vacancies = vacancies.Where(x => x.EmploymentType.ToLower() == model.EmploymentType.ToLower());
            if (model.RemoteWork is not null)
                vacancies = vacancies.Where(x => x.RemoteWork == model.RemoteWork);
            if (model.VacancyCities is not null)
                vacancies = vacancies.Where(x => model.VacancyCities.Contains(x.VacancyCity));

            vacancies = vacancies.Where(x =>
                x.Position.ToLower().Contains(searchingQuery) | x.CompanyName.ToLower().Contains(searchingQuery));

            return await vacancies.Skip(currentPageNumber * PaginationConstants.VacancyPageSize).CountAsync() > 0;
        }

        public async Task<bool> DoesNextVacanciesByCompanyIdPageExist(Guid companyId, int currentPageNumber)
          => await context.Vacancies.Where(x => x.CompanyId == companyId)
              .Skip(currentPageNumber * PaginationConstants.VacancyPageSize)
              .CountAsync() > 0;
        
    }
}
