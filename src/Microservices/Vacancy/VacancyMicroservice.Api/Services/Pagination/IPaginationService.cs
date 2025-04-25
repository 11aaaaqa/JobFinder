using VacancyMicroservice.Api.DTOs;

namespace VacancyMicroservice.Api.Services.Pagination
{
    public interface IPaginationService
    {
        Task<bool> DoesNextAllVacanciesPageExist(int currentPageNumber);
        Task<bool> DoesNextSearchVacanciesPageExist(string searchingQuery, int currentPageNumber);
        Task<bool> DoesNextFilteredVacanciesPageExist(GetFilteredVacanciesDto model, int currentPageNumber);
        Task<bool> DoesNextSearchFilteredVacanciesPageExist(GetFilteredVacanciesDto model, string searchingQuery, int currentPageNumber);
        Task<bool> DoesNextVacanciesByCompanyIdPageExist(Guid companyId, int currentPageNumber, string? searchingQuery);
    }
}
