using EmployerMicroservice.Api.Constants;
using EmployerMicroservice.Api.Database;
using EmployerMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployerMicroservice.Api.Services.Searching_services
{
    public class SearchingService(ApplicationDbContext context) : ISearchingService
    {
        public async Task<List<Employer>> FindEmployersAsync(Guid companyId,int pageNumber, string query)
        {
            var lowerQuery = query.ToLower();
            var employers = await context.Employers.Where(x => x.CompanyId == companyId)
                .Where(x => x.Name.ToLower().Contains(lowerQuery) | x.Surname.ToLower().Contains(lowerQuery) |
                            x.Email.ToLower().Contains(lowerQuery) | x.CompanyPost.ToLower().Contains(lowerQuery))
                .Skip((pageNumber - 1) * PaginationConstants.CompanyEmployersCountConstant)
                .Take(PaginationConstants.CompanyEmployersCountConstant)
                .ToListAsync();
            return employers;
        }
    }
}
