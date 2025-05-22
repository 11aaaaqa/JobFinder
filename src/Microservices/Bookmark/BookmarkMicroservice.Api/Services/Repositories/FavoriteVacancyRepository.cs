using BookmarkMicroservice.Api.Constants;
using BookmarkMicroservice.Api.Database;
using BookmarkMicroservice.Api.DTOs;
using BookmarkMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookmarkMicroservice.Api.Services.Repositories
{
    public class FavoriteVacancyRepository(ApplicationDbContext context) : IFavoriteVacancyRepository

    {
        public async Task<List<FavoriteVacancy>> GetFavoriteVacanciesByEmployeeIdAsync(Guid employeeId, int pageNumber, string? searchingQuery)
        {
            var vacancies = context.FavoriteVacancies.Where(x => x.EmployeeId == employeeId).AsQueryable();
            if (searchingQuery is not null)
                vacancies = vacancies.Where(x => x.Position.ToLower().Contains(searchingQuery.ToLower()));

            return await vacancies.Skip((pageNumber - 1) * PaginationConstants.FavouriteVacanciesPageSize)
                .Take(PaginationConstants.FavouriteVacanciesPageSize).ToListAsync();
        }

        public async Task AddToFavoritesAsync(AddVacancyDto model)
        {
            await context.FavoriteVacancies.AddAsync(new FavoriteVacancy
            {
                Id = Guid.NewGuid(), CompanyName = model.CompanyName, SalaryTo = model.SalaryTo, Position = model.Position,
                SalaryFrom = model.SalaryFrom, WorkExperience = model.WorkExperience, EmployeeId = model.EmployeeId,
                VacancyCity = model.VacancyCity, VacancyId = model.VacancyId
            });
            await context.SaveChangesAsync();
        }

        public async Task DeleteFromFavoritesAsync(Guid vacancyId, Guid employeeId)
        {
            var vacancy = await context.FavoriteVacancies.Where(x => x.EmployeeId == employeeId)
                .SingleOrDefaultAsync(x => x.VacancyId == vacancyId);

            if (vacancy is null)
                return;

            context.FavoriteVacancies.Remove(vacancy);
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsVacancyInFavouritesAsync(Guid vacancyId, Guid employeeId)
        {
            var vacancy = await context.FavoriteVacancies.Where(x => x.EmployeeId == employeeId)
                .SingleOrDefaultAsync(x => x.VacancyId == vacancyId);

            return vacancy == null;
        }
    }
}
