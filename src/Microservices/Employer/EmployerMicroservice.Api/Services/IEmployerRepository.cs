using EmployerMicroservice.Api.DTOs;
using EmployerMicroservice.Api.Models;

namespace EmployerMicroservice.Api.Services
{
    public interface IEmployerRepository
    {
        Task<Employer?> GetEmployerByIdAsync(Guid id);
        Task<Employer?> GetEmployerByEmailAsync(string email);
        Task<List<Employer>> GetEmployersByCompanyId(Guid companyId, int pageNumber);
        Task<bool> UpdateEmployerAsync(UpdateEmployerDto model);
        Task<bool> AssignCompanyAsync(Guid employerId, Guid companyId);
    }
}
