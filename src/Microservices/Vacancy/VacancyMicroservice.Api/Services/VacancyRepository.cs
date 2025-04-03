using Microsoft.EntityFrameworkCore;
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
                                                               | x.Position.ToLower().Contains(searchingQuery) |
                                                               x.Profession.ToLower().Contains(searchingQuery))
                .Skip((pageNumber - 1) * PaginationConstants.VacancyPageSize)
                .Take(PaginationConstants.VacancyPageSize)
                .ToListAsync();
            return vacancies;
        }

        public async Task<List<Vacancy>> GetFilteredVacanciesAsync(string? profession, string? position, int? salaryFrom,
            string? workExperience, string? employmentType, bool? remoteWork, List<string>? vacancyCities, int pageNumber)
        {
            var vacancies = context.Vacancies.AsQueryable();
            if (profession is not null)
                vacancies = vacancies.Where(x => x.Profession.ToLower() == profession.ToLower());
            if (position is not null)
                vacancies = vacancies.Where(x => x.Position.ToLower() == position.ToLower());
            if (salaryFrom is not null)
                vacancies = vacancies.Where(x => x.SalaryTo >= salaryFrom);
            if (workExperience is not null)
                vacancies = vacancies.Where(x => x.WorkExperience.ToLower() == workExperience);
            if (employmentType is not null)
                vacancies = vacancies.Where(x => x.EmploymentType.ToLower() == employmentType.ToLower());
            if (remoteWork is not null)
                vacancies = vacancies.Where(x => x.RemoteWork == remoteWork);
            if (vacancyCities is not null)
                vacancies = vacancies.Where(x => vacancyCities.Contains(x.VacancyCity));

            return await vacancies.Skip((pageNumber - 1) * PaginationConstants.VacancyPageSize)
                .Take(PaginationConstants.VacancyPageSize).ToListAsync();
        }

        public async Task<List<Vacancy>> GetVacanciesByCompanyIdAsync(Guid companyId, int pageNumber)
            => await context.Vacancies.Where(x => x.CompanyId == companyId)
                .Skip((pageNumber - 1) * PaginationConstants.VacancyPageSize).Take(PaginationConstants.VacancyPageSize)
                .ToListAsync();

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
                CompanyName = vacancy.CompanyName,
                Position = model.Position, Description = model.Description,
                EmployerContactEmail = model.EmployerContactEmail,
                EmployerContactPhoneNumber = model.EmployerContactPhoneNumber, EmploymentType = model.EmploymentType,
                Profession = model.Profession, RemoteWork = model.RemoteWork, SalaryFrom = model.SalaryFrom,
                SalaryTo = model.SalaryTo,
                VacancyCity = model.VacancyCity, WorkExperience = model.WorkExperience,
                WorkerResponsibilities = model.WorkerResponsibilities
            };
            context.Vacancies.Update(updatedVacancy);
            await context.SaveChangesAsync();
        }
    }
}
