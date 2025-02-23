using CompanyMicroservice.Api.Database;
using CompanyMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyMicroservice.Api.Services
{
    public class CompanyEmployerRepository(ApplicationDbContext context) : ICompanyEmployerRepository
    {
        public async Task RemoveEmployerFromCompanyAsync(Guid companyId, Guid employerId)
        {
            var company = await context.Companies.SingleAsync(x => x.Id == companyId);

            company.CompanyEmployersIds.Remove(employerId);
            await context.SaveChangesAsync();
        }

        public async Task RequestJoiningCompanyAsync(Guid companyId, Guid employerId, string employerName, string employerSurname)
        {
            await context.JoiningRequestedEmployers.AddAsync(new JoiningRequestedEmployer
            {
                Id = Guid.NewGuid(), CompanyId = companyId, EmployerId = employerId, EmployerName = employerName, EmployerSurname = employerSurname,
                JoiningRequestDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        public async Task<bool> DidEmployerAlreadyRequestJoiningAsync(Guid employerId, Guid companyId)
        {
            var joiningRequest = await context.JoiningRequestedEmployers.Where(x => x.CompanyId == companyId)
                .SingleOrDefaultAsync(x => x.EmployerId == employerId);
            if (joiningRequest is null) return false;
            return true;
        }
    }
}
