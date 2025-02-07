using EmployeeMicroservice.Api.Database;
using EmployeeMicroservice.Api.DTOs;
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

        public async Task UpdateEmployeeAsync(UpdateEmployeeDto model)
        {
            var employee = await context.Employees.SingleAsync(x => x.Id == model.Id);

            employee.Name = model.Name; employee.Surname = model.Surname; employee.Patronymic = model.Patronymic; employee.Gender = model.Gender;
            employee.DateOfBirth = model.DateOfBirth; employee.City = model.City; employee.PhoneNumber = model.PhoneNumber;
            employee.Status = model.Status;

            await context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeStatusAsync(Guid employeeId, string status)
        {
            var user = await context.Employees.SingleAsync(x => x.Id == employeeId);

            user.Status = status;

            await context.SaveChangesAsync();
        }
    }
}
