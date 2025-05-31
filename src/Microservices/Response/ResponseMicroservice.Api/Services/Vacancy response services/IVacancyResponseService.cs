using ResponseMicroservice.Api.Enums;
using ResponseMicroservice.Api.Models;

namespace ResponseMicroservice.Api.Services.Vacancy_response_services
{
    public interface IVacancyResponseService
    {
        Task<List<VacancyResponse>> GetVacancyResponsesByEmployeeIdAsync(Guid employeeId, string? searchingQuery,
            DateTimeOrderByType orderByTimeType, int pageNumber);
        Task<List<VacancyResponse>> GetVacancyResponsesByCompanyIdAsync(Guid companyId, DateTimeOrderByType orderByTimeType, int pageNumber);
        Task<List<VacancyResponse>> GetCompanyVacancyResponsesByVacancyIdAsync(Guid vacancyId, DateTimeOrderByType orderByTimeType, int pageNumber);
        Task SetVacancyResponseStatusAsync(Guid vacancyResponseId, string status);
        Task AddVacancyResponseAsync(VacancyResponse model);
        Task<bool> HasEmployeeRespondedToVacancyAsync(Guid employeeId, Guid vacancyId);
    }
}
