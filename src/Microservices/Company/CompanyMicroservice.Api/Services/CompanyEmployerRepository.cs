using CompanyMicroservice.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace CompanyMicroservice.Api.Services
{
    public class CompanyEmployerRepository(ApplicationDbContext context) : ICompanyEmployerRepository
    {
        public async Task RemoveEmployerFromCompany(Guid companyId, Guid employerId)
        {
            var company = await context.Companies.SingleAsync(x => x.Id == companyId);

            company.CompanyEmployersIds.Remove(employerId);
            await context.SaveChangesAsync();
        }
    }
}
