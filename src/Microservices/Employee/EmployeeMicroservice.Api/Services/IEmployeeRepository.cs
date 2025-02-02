using EmployeeMicroservice.Api.Models;

namespace EmployeeMicroservice.Api.Services
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetEmployeeByIdAsync(Guid id);
        Task<Employee?> GetEmployeeByEmailAsync(string email);
        Task UpdateEmployeeAsync(Employee employee);
    }
}
