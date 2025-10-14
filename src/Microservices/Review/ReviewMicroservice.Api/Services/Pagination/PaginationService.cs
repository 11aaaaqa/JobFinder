using Microsoft.EntityFrameworkCore;
using ReviewMicroservice.Api.Constants;
using ReviewMicroservice.Api.Database;

namespace ReviewMicroservice.Api.Services.Pagination
{
    public class PaginationService(ApplicationDbContext context) : IPaginationService
    {
        public async Task<bool> IsNextReviewsByCompanyIdPageExistedAsync(Guid companyId, int currentPageNumber)
        {
            return await context.Reviews.Where(x => x.CompanyId == companyId)
                .Skip(PaginationConstants.ReviewsPageSize * currentPageNumber).CountAsync() > 0;
        }

        public async Task<bool> IsNextReviewsByEmployeeIdPageExistedAsync(Guid employeeId, int currentPageNumber)
        {
            return await context.Reviews.Where(x => x.EmployeeId == employeeId)
                .Skip(PaginationConstants.ReviewsPageSize * currentPageNumber).CountAsync() > 0;
        }
    }
}
