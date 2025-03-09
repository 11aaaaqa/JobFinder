using EmployerMicroservice.Api.Constants;
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

        public async Task<List<Employer>> GetEmployersByCompanyId(Guid companyId, int pageNumber)
        {
            var employers = await context.Employers.Where(x => x.CompanyId == companyId)
                .Skip((pageNumber - 1) * PaginationConstants.CompanyEmployersCountConstant)
                .Take(PaginationConstants.CompanyEmployersCountConstant).ToListAsync();
            return employers;
        }

        public async Task<bool> UpdateEmployerAsync(UpdateEmployerDto model)
        {
            var employer = await context.Employers.SingleOrDefaultAsync(x => x.Id == model.Id);
            if (employer is null) return false;

            employer.Name = model.Name; employer.Surname = model.Surname; employer.CompanyPost = model.CompanyPost;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignCompanyAsync(Guid employerId, Guid companyId)
        {
            var employer = await context.Employers.SingleOrDefaultAsync(x => x.Id == employerId);
            if (employer is null) return false;

            employer.CompanyId = companyId;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
