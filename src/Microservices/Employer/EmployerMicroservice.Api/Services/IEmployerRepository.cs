using EmployerMicroservice.Api.DTOs;
using EmployerMicroservice.Api.Models;

namespace EmployerMicroservice.Api.Services
{
    public interface IEmployerRepository
    {
        Task<Employer?> GetEmployerByIdAsync(Guid id);
        Task<Employer?> GetEmployerByEmailAsync(string email);
        Task UpdateEmployerAsync(UpdateEmployerDto model);
        Task AssignCompanyAsync(Guid employerId, Guid companyId);
    }
}
