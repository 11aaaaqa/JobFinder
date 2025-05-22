using BookmarkMicroservice.Api.DTOs;
using BookmarkMicroservice.Api.Models;

namespace BookmarkMicroservice.Api.Services.Repositories
{
    public interface IFavoriteVacancyRepository
    {
        Task<List<FavoriteVacancy>> GetFavoriteVacanciesByEmployeeIdAsync(Guid employeeId, int pageNumber, string? searchingQuery);
        Task AddToFavoritesAsync(AddVacancyDto model);
        Task DeleteFromFavoritesAsync(Guid vacancyId, Guid employeeId);
        Task<bool> IsVacancyInFavouritesAsync(Guid vacancyId, Guid employeeId);
    }
}
