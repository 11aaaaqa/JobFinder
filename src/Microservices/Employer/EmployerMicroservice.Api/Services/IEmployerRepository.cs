using EmployerMicroservice.Api.DTOs;
using EmployerMicroservice.Api.Models;

namespace EmployerMicroservice.Api.Services
{
    public interface IEmployerRepository
    {
        Task<Employer?> GetEmployerByIdAsync(Guid id);
        Task<Employer?> GetEmployerByEmailAsync(string email);
        Task<bool> UpdateEmployerAsync(UpdateEmployerDto model);
        Task<bool> AssignCompanyAsync(Guid employerId, Guid companyId);
    }
}
