using EmployeeMicroservice.Api.Database;
using EmployeeMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMicroservice.Api.Services
{
    public class EmployeeRepository(ApplicationDbContext context) : IEmployeeRepository
    {
        public async Task<Employee?> GetEmployeeByIdAsync(Guid id)
            => await context.Employees.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
            => await context.Employees.SingleOrDefaultAsync(x => x.Email == email);

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            context.Employees.Update(employee);
            await context.SaveChangesAsync();
        }
    }
}
