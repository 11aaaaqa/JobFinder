using VacancyMicroservice.Api.DTOs;
using VacancyMicroservice.Api.Models;

namespace VacancyMicroservice.Api.Services
{
    public interface IVacancyRepository
    {
        Task<Vacancy?> GetVacancyByIdAsync(Guid vacancyId);
        Task<List<Vacancy>> GetAllVacanciesAsync(int pageNumber);
        Task<List<Vacancy>> SearchVacanciesAsync(string searchingQuery, int pageNumber);
        Task<List<Vacancy>> GetFilteredVacanciesAsync(GetFilteredVacanciesDto model, int pageNumber);
        Task<List<Vacancy>> SearchFilteredVacanciesAsync(GetFilteredVacanciesDto model, string searchingQuery, int pageNumber);
        Task<List<Vacancy>> GetVacanciesByCompanyIdAsync(Guid companyId, int pageNumber, string? searchingQuery);
        Task AddVacancyAsync(Vacancy vacancy);
        Task DeleteVacancyAsync(Guid vacancyId);
        Task UpdateVacancyAsync(UpdateVacancyDto model);
        Task ArchiveVacancyAsync(Guid vacancyId);
        Task UnarchiveVacancyAsync(Guid vacancyId);
        Task<List<Vacancy>> GetArchivedVacanciesByCompanyIdAsync(Guid companyId, int pageNumber, string? searchingQuery);
    }
}
