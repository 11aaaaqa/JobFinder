using CompanyMicroservice.Api.Database;
using CompanyMicroservice.Api.Exceptions;
using CompanyMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyMicroservice.Api.Services
{
    public class JoinToCompanyRepository(ApplicationDbContext context) : IJoinToCompanyRepository
    {
        public async Task<List<JoiningRequestedEmployer>> GetJoiningRequestedEmployersByCompanyIdAsync(Guid companyId)
            => await context.JoiningRequestedEmployers.Where(x => x.CompanyId == companyId).ToListAsync();

        public async Task AddJoiningRequestAsync(JoiningRequestedEmployer model)
        {
            await context.JoiningRequestedEmployers.AddAsync(model);
            await context.SaveChangesAsync();
        }

        public async Task AcceptRequestAsync(Guid requestId)
        {
            var joiningRequestedEmployer =
                await context.JoiningRequestedEmployers.SingleAsync(x => x.Id == requestId);

           var company = await context.Companies.SingleAsync(x => x.Id == joiningRequestedEmployer.CompanyId);
           if (company.CompanyEmployersIds.Contains(joiningRequestedEmployer.EmployerId))
               throw new EmployerIsAlreadyInCompanyException();

           company.CompanyEmployersIds.Add(joiningRequestedEmployer.EmployerId);
           context.JoiningRequestedEmployers.Remove(joiningRequestedEmployer);

           await context.SaveChangesAsync();
        }

        public async Task RejectRequestAsync(Guid requestId)
        {
            context.JoiningRequestedEmployers.Remove(new JoiningRequestedEmployer { Id = requestId });
            await context.SaveChangesAsync();
        }
    }
}
