using VacancyMicroservice.Api.DTOs;
using VacancyMicroservice.Api.Models;

namespace VacancyMicroservice.Api.Services
{
    public interface IVacancyRepository
    {
        Task<Vacancy?> GetVacancyByIdAsync(Guid vacancyId);
        Task<List<Vacancy>> GetAllVacanciesAsync(int pageNumber);
        Task<List<Vacancy>> SearchVacanciesAsync(string searchingQuery, int pageNumber);
        Task<List<Vacancy>> GetFilteredVacanciesAsync(string profession, string? position, int? salaryFrom, int? salaryTo, string? workExperience,
            string? employmentType, bool? remoteWork, List<string>? vacancyCities, int pageNumber);
        Task<List<Vacancy>> GetVacanciesByCompanyIdAsync(Guid companyId, int pageNumber);
        Task AddVacancyAsync(Vacancy vacancy);
        Task DeleteVacancyAsync(Guid vacancyId);
        Task UpdateVacancyAsync(UpdateVacancyDto model);
    }
}
