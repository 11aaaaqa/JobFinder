using BookmarkMicroservice.Api.DTOs;
using BookmarkMicroservice.Api.Services.Pagination;
using BookmarkMicroservice.Api.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookmarkMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteVacancyController(IFavoriteVacancyRepository favoriteVacancyRepository,
        ICheckForNextPageExistingService paginationService) : ControllerBase
    {
        [HttpGet]
        [Route("GetFavoriteVacancies/{employeeId}")]
        public async Task<IActionResult> GetFavoriteVacanciesAsync(Guid employeeId, string? searchingQuery, int pageNumber)
            => Ok(await favoriteVacancyRepository.GetFavoriteVacanciesByEmployeeIdAsync(employeeId, pageNumber, searchingQuery));

        [HttpGet]
        [Route("DoesNextFavoriteVacanciesPageExist/{employeeId}")]
        public async Task<IActionResult> DoesNextFavoriteVacanciesPageExistAsync(Guid employeeId, 
            string? searchingQuery, int currentPageNumber)
            => Ok(await paginationService.DoesNextFavoriteVacanciesByEmployeeIdPageExistAsync(employeeId, searchingQuery, currentPageNumber));

        [HttpPost]
        [Route("AddToFavorites")]
        public async Task<IActionResult> AddToFavoritesAsync([FromBody] AddVacancyDto model)
        {
            await favoriteVacancyRepository.AddToFavoritesAsync(model);
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteFromFavorites")]
        public async Task<IActionResult> DeleteFromFavoritesAsync(Guid employeeId, Guid vacancyId)
        {
            await favoriteVacancyRepository.DeleteFromFavoritesAsync(vacancyId, employeeId);
            return Ok();
        }

        [HttpGet]
        [Route("IsVacancyInFavorites")]
        public async Task<IActionResult> IsVacancyInFavoritesAsync(Guid employeeId, Guid vacancyId)
            => Ok(await favoriteVacancyRepository.IsVacancyInFavouritesAsync(vacancyId, employeeId));
    }
}
