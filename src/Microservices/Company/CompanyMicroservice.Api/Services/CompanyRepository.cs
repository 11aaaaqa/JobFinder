using CompanyMicroservice.Api.Database;
using CompanyMicroservice.Api.DTOs;
using CompanyMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyMicroservice.Api.Services
{
    public class CompanyRepository(ApplicationDbContext context) : ICompanyRepository
    {
        public async Task<Company?> GetCompanyByIdAsync(Guid id)
            => await context.Companies.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<Company?> GetCompanyByCompanyNameAsync(string companyName)
            => await context.Companies.SingleOrDefaultAsync(x => x.CompanyName.ToLower() == companyName.ToLower());

        public async Task AddCompanyAsync(Company model)
        {
            await context.Companies.AddAsync(model);
            await context.SaveChangesAsync();
        }

        public async Task<bool> UpdateCompanyAsync(UpdateCompanyDto model)
        {
            var company = await context.Companies.SingleOrDefaultAsync(x => x.Id == model.Id);
            if (company is null) return false;

            company.CompanyDescription = model.CompanyDescription; company.CompanyName = model.CompanyName;
            company.CompanyColleaguesCount = model.CompanyColleaguesCount;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteCompanyAsync(Guid id)
        {
            context.Companies.Remove(new Company{Id = id});
            await context.SaveChangesAsync();
        }
    }
}
