using EmployerMicroservice.Api.Database;
using EmployerMicroservice.Api.DTOs;
using EmployerMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployerMicroservice.Api.Services
{
    public class CompanyRepository(ApplicationDbContext context) : ICompanyRepository
    {
        public async Task<Company?> GetCompanyByIdAsync(Guid id)
            => await context.Companies.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<Company?> GetCompanyByCompanyNameAsync(string companyName)
            => await context.Companies.SingleOrDefaultAsync(x => x.CompanyName == companyName);

        public async Task AddCompanyAsync(Company model)
        {
            await context.Companies.AddAsync(model);
            await context.SaveChangesAsync();
        }

        public async Task UpdateCompanyAsync(UpdateCompanyDto model)
        {
            var company = await context.Companies.SingleOrDefaultAsync(x => x.Id == model.Id);
            if (company is null) throw new Exception("Company with current ID wasn't found");

            company.CompanyDescription = model.CompanyDescription; company.CompanyName = model.CompanyName;
            company.CompanyColleaguesCount = model.CompanyColleaguesCount;

            await context.SaveChangesAsync();
        }

        public async Task DeleteCompanyAsync(Guid id)
        {
            context.Companies.Remove(new Company{Id = id});
            await context.SaveChangesAsync();
        }
    }
}
