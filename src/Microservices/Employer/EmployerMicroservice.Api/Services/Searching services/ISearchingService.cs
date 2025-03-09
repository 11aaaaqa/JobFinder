using EmployerMicroservice.Api.Models;

namespace EmployerMicroservice.Api.Services.Searching_services
{
    public interface ISearchingService
    {
        Task<List<Employer>> FindEmployersAsync(Guid companyId,int pageNumber, string query);
    }
}
