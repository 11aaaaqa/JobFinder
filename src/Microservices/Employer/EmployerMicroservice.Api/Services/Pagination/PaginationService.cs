using EmployerMicroservice.Api.Constants;
using EmployerMicroservice.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace EmployerMicroservice.Api.Services.Pagination
{
    public class PaginationService(ApplicationDbContext context) : IPaginationService
    {
        public async Task<bool> DoesNextEmployersByCompanyPageExist(Guid companyId, int currentPageNumber)
        {
            var remainingEmployersCount = await context.Employers.Where(x => x.CompanyId == companyId)
                .Skip(currentPageNumber * PaginationConstants.CompanyEmployersCountConstant).CountAsync();
            return remainingEmployersCount > 0;
        }

        public async Task<bool> DoesNextSearchingEmployersPageExist(Guid companyId, int currentPageNumber, string searchingQuery)
        {
            var lowerQuery = searchingQuery.ToLower();
            var remainingEmployersCount = await context.Employers.Where(x => x.CompanyId == companyId)
                .Where(x => x.Name.ToLower().Contains(lowerQuery) | x.Surname.ToLower().Contains(lowerQuery) |
                            x.Email.ToLower().Contains(lowerQuery) | x.CompanyPost.ToLower().Contains(lowerQuery))
                .Skip(currentPageNumber * PaginationConstants.CompanyEmployersCountConstant).CountAsync();
            return remainingEmployersCount > 0;
        }
    }
}
