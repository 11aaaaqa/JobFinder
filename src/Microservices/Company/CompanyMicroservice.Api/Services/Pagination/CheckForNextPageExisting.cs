using CompanyMicroservice.Api.Constants;
using CompanyMicroservice.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace CompanyMicroservice.Api.Services.Pagination
{
    public class CheckForNextPageExisting(ApplicationDbContext context) : ICheckForNextPageExisting
    {
        public async Task<bool> DoesEmployersRequestedJoiningPageExist(Guid companyId, int pageNumber)
        {
            int remainingCount = await context.JoiningRequestedEmployers.Where(x => x.CompanyId == companyId)
                .Skip((pageNumber - 1) * PaginationConstants.EmployersPageSize).CountAsync();
            return remainingCount > 0;
        }
    }
}
