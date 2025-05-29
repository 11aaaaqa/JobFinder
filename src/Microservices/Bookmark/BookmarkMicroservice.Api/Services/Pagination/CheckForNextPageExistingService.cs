using BookmarkMicroservice.Api.Constants;
using BookmarkMicroservice.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace BookmarkMicroservice.Api.Services.Pagination
{
    public class CheckForNextPageExistingService(ApplicationDbContext context) : ICheckForNextPageExistingService
    {
        public async Task<bool> DoesNextFavoriteVacanciesByEmployeeIdPageExistAsync(Guid employeeId, string? searchingQuery,
            int currentPageNumber)
        {
            var vacancies = context.FavoriteVacancies.Where(x => x.EmployeeId == employeeId).AsQueryable();
            if (searchingQuery is not null)
                vacancies = vacancies.Where(x => x.Position.ToLower().Contains(searchingQuery.ToLower()));

            return await vacancies.Skip(currentPageNumber * PaginationConstants.FavouriteVacanciesPageSize).CountAsync() > 0;
        }
    }
}
