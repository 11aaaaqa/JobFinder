using EmployerMicroservice.Api.DTOs;
using EmployerMicroservice.Api.Models;

namespace EmployerMicroservice.Api.Services
{
    public interface ICompanyRepository
    {
        Task<Company?> GetCompanyByIdAsync(Guid id);
        Task<Company?> GetCompanyByCompanyNameAsync(string companyName);
        Task AddCompanyAsync(Company model);
        Task<bool> UpdateCompanyAsync(UpdateCompanyDto model);
        Task DeleteCompanyAsync(Guid id);
    }
}
