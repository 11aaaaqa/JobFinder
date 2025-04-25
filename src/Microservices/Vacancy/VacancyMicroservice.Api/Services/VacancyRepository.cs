using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VacancyMicroservice.Api.Constants;
using VacancyMicroservice.Api.Database;
using VacancyMicroservice.Api.DTOs;
using VacancyMicroservice.Api.Models;

namespace VacancyMicroservice.Api.Services
{
    public class VacancyRepository(ApplicationDbContext context) : IVacancyRepository
    {
        public async Task<Vacancy?> GetVacancyByIdAsync(Guid vacancyId)
            => await context.Vacancies.SingleOrDefaultAsync(x => x.Id == vacancyId);

        public async Task<List<Vacancy>> GetAllVacanciesAsync(int pageNumber)
            => await context.Vacancies
                .Skip((pageNumber - 1) * PaginationConstants.VacancyPageSize)
                .Take(PaginationConstants.VacancyPageSize)
                .ToListAsync();

        public async Task<List<Vacancy>> SearchVacanciesAsync(string searchingQuery, int pageNumber)
        {
            searchingQuery = searchingQuery.ToLower();
            var vacancies = await context.Vacancies.Where(x => x.CompanyName.ToLower().Contains(searchingQuery)
                                                               | x.Position.ToLower().Contains(searchingQuery))
                .Skip((pageNumber - 1) * PaginationConstants.VacancyPageSize)
                .Take(PaginationConstants.VacancyPageSize)
                .ToListAsync();
            return vacancies;
        }

        public async Task<List<Vacancy>> GetFilteredVacanciesAsync(GetFilteredVacanciesDto model, int pageNumber)
        {
            var vacancies = context.Vacancies.AsQueryable();
            if (model.Position is not null)
                vacancies = vacancies.Where(x => x.Position.ToLower().Contains(model.Position.ToLower()));
            if (model.SalaryFrom is not null)
                vacancies = vacancies.Where(x => x.SalaryTo >= model.SalaryFrom | (x.SalaryTo == null && x.SalaryFrom >= model.SalaryFrom));
            if (model.WorkExperience is not null)
                vacancies = vacancies.Where(x => x.WorkExperience.ToLower().Contains(model.WorkExperience.ToLower()));
            if (model.EmploymentType is not null)
                vacancies = vacancies.Where(x => x.EmploymentType.ToLower().Contains(model.EmploymentType.ToLower()));
            if (model.RemoteWork is not null)
                vacancies = vacancies.Where(x => x.RemoteWork == model.RemoteWork);
            if (model.VacancyCities is not null)
                vacancies = vacancies.Where(x => model.VacancyCities.Contains(x.VacancyCity));

            return await vacancies.Skip((pageNumber - 1) * PaginationConstants.VacancyPageSize)
                .Take(PaginationConstants.VacancyPageSize).ToListAsync();
        }

        public async Task<List<Vacancy>> SearchFilteredVacanciesAsync(GetFilteredVacanciesDto model, string searchingQuery, int pageNumber)
        {
            var vacancies = context.Vacancies.AsQueryable();
            searchingQuery = searchingQuery.ToLower();
            if (model.Position is not null)
                vacancies = vacancies.Where(x => x.Position.ToLower().Contains(model.Position.ToLower()));
            if (model.SalaryFrom is not null)
                vacancies = vacancies.Where(x => x.SalaryTo >= model.SalaryFrom | (x.SalaryTo == null && x.SalaryFrom >= model.SalaryFrom));
            if (model.WorkExperience is not null)
                vacancies = vacancies.Where(x => x.WorkExperience.ToLower().Contains(model.WorkExperience.ToLower()));
            if (model.EmploymentType is not null)
                vacancies = vacancies.Where(x => x.EmploymentType.ToLower().Contains(model.EmploymentType.ToLower()));
            if (model.RemoteWork is not null)
                vacancies = vacancies.Where(x => x.RemoteWork == model.RemoteWork);
            if (model.VacancyCities is not null)
                vacancies = vacancies.Where(x => model.VacancyCities.Contains(x.VacancyCity));

            vacancies = vacancies.Where(x =>
                x.Position.ToLower().Contains(searchingQuery) | x.CompanyName.ToLower().Contains(searchingQuery));

            return await vacancies.Skip((pageNumber - 1) * PaginationConstants.VacancyPageSize)
                .Take(PaginationConstants.VacancyPageSize).ToListAsync();
        }

        public async Task<List<Vacancy>> GetVacanciesByCompanyIdAsync(Guid companyId, int pageNumber, string? searchingQuery)
        {
            var vacancies = context.Vacancies.Where(x => x.CompanyId == companyId).AsQueryable();

            if (searchingQuery is not null)
                vacancies = vacancies.Where(x => x.Position.ToLower().Contains(searchingQuery.ToLower()));

            return await vacancies.Skip((pageNumber - 1) * PaginationConstants.VacancyPageSize)
                .Take(PaginationConstants.VacancyPageSize).ToListAsync();
        }

        public async Task AddVacancyAsync(Vacancy vacancy)
        {
            context.Vacancies.Add(vacancy);
            await context.SaveChangesAsync();
        }

        public async Task DeleteVacancyAsync(Guid vacancyId)
        {
            var vacancy = await context.Vacancies.SingleOrDefaultAsync(x => x.Id == vacancyId);
            if (vacancy is null)
                return;
            context.Vacancies.Remove(vacancy);
            await context.SaveChangesAsync();
        }

        public async Task UpdateVacancyAsync(UpdateVacancyDto model)
        {
            var vacancy = await context.Vacancies.SingleAsync(x => x.Id == model.Id);
            var updatedVacancy = new Vacancy
            {
                Id = model.Id, Address = model.Address, CompanyId = vacancy.CompanyId,
                CompanyName = vacancy.CompanyName, Position = model.Position, Description = model.Description, 
                EmployerContactEmail = model.EmployerContactEmail,
                EmployerContactPhoneNumber = model.EmployerContactPhoneNumber, EmploymentType = model.EmploymentType,
                RemoteWork = model.RemoteWork, SalaryFrom = model.SalaryFrom, SalaryTo = model.SalaryTo,
                VacancyCity = model.VacancyCity, WorkExperience = model.WorkExperience
            };
            context.Vacancies.Update(updatedVacancy);
            await context.SaveChangesAsync();
        }
    }
}
