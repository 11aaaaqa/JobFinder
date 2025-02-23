using CompanyMicroservice.Api.Constants;
using CompanyMicroservice.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace CompanyMicroservice.Api.Services.Pagination
{
    public class CheckForNextPageExisting(ApplicationDbContext context) : ICheckForNextPageExisting
    {
        public async Task<bool> DoesNextEmployersRequestedJoiningPageExist(Guid companyId, int pageNumber)
        {
            int startingCount = await context.JoiningRequestedEmployers.Where(x => x.CompanyId == companyId)
                .Skip((pageNumber - 1) * PaginationConstants.EmployersPageSize).CountAsync();
            return startingCount > 0;
        }
    }
}
