using CompanyMicroservice.Api.Constants;
using CompanyMicroservice.Api.Database;
using CompanyMicroservice.Api.Exceptions;
using CompanyMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyMicroservice.Api.Services
{
    public class CompanyEmployerRepository(ApplicationDbContext context) : ICompanyEmployerRepository
    {
        public async Task<List<JoiningRequestedEmployer>> GetListOfEmployersRequestedJoiningAsync(Guid companyId, int pageNumber)
        {
            var employers = await context.JoiningRequestedEmployers.Where(x => x.CompanyId == companyId)
                .Skip(PaginationConstants.EmployersPageSize * (pageNumber - 1)).Take(PaginationConstants.EmployersPageSize).ToListAsync();
            return employers;
        }

        public async Task RemoveEmployerFromCompanyAsync(Guid companyId, Guid employerId)
        {
            var company = await context.Companies.SingleAsync(x => x.Id == companyId);

            company.CompanyEmployersIds.Remove(employerId);
            await context.SaveChangesAsync();
        }

        public async Task RemoveAllEmployerRequestsAsync(Guid employerId)
        {
            var requests = await context.JoiningRequestedEmployers
                .Where(x => x.EmployerId == employerId).ToListAsync();
            context.JoiningRequestedEmployers.RemoveRange(requests);
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

        public async Task DeleteEmployerJoiningAsync(Guid joiningRequestId)
        {
            var request = await context.JoiningRequestedEmployers.SingleAsync(x => x.Id == joiningRequestId);
            context.JoiningRequestedEmployers.Remove(request);
            await context.SaveChangesAsync();
        }

        public async Task<JoiningRequestedEmployer?> GetJoiningRequestByRequestId(Guid id)
            => await context.JoiningRequestedEmployers.SingleOrDefaultAsync(x => x.Id == id);

        public async Task AddEmployerToCompany(Guid companyId, Guid employerId)
        {
            var company = await context.Companies.SingleAsync(x => x.Id == companyId);

            if (company.CompanyEmployersIds.Contains(employerId))
                throw new EmployerIsAlreadyInCompanyException();

            company.CompanyEmployersIds.Add(employerId);
            await context.SaveChangesAsync();
        }
    }
}
