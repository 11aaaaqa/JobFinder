using EmployerMicroservice.Api.Database;
using EmployerMicroservice.Api.DTOs;
using EmployerMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployerMicroservice.Api.Services
{
    public class EmployerRepository(ApplicationDbContext context) : IEmployerRepository
    {
        public async Task<Employer?> GetEmployerByIdAsync(Guid id)
            => await context.Employers.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<Employer?> GetEmployerByEmailAsync(string email)
            => await context.Employers.SingleOrDefaultAsync(x => x.Email == email);

        public async Task UpdateEmployerAsync(UpdateEmployerDto model)
        {
            var employer = await context.Employers.SingleOrDefaultAsync(x => x.Id == model.Id);
            if (employer is null) throw new Exception("Employer with current ID wasn't found");

            employer.Name = model.Name; employer.Surname = model.Surname; employer.CompanyPost = model.CompanyPost;

            await context.SaveChangesAsync();
        }

        public async Task AssignCompanyAsync(Guid employerId, Guid companyId)
        {
            var employer = await context.Employers.SingleOrDefaultAsync(x => x.Id == employerId);
            if (employer is null) throw new Exception("Employer with current ID wasn't found");

            employer.CompanyId = companyId;

            await context.SaveChangesAsync();
        }
    }
}
