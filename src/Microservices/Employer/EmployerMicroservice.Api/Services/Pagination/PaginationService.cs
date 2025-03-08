using EmployerMicroservice.Api.Constants;
using EmployerMicroservice.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace EmployerMicroservice.Api.Services.Pagination
{
    public class PaginationService(ApplicationDbContext context) : IPaginationService
    {
        public async Task<bool> DoesNextEmployersByCompanyPageExist(Guid companyId, int currentPageNumber)
        {
            var remainedEmployersCount = await context.Employers.Where(x => x.CompanyId == companyId)
                .Skip(currentPageNumber * PaginationConstants.CompanyEmployersConstant).CountAsync();
            return remainedEmployersCount > 0;
        }
    }
}
