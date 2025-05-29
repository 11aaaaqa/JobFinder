using BookmarkMicroservice.Api.Controllers;
using BookmarkMicroservice.Api.DTOs;
using BookmarkMicroservice.Api.Models;
using BookmarkMicroservice.Api.Services.Pagination;
using BookmarkMicroservice.Api.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ICheckForNextPageExistingService = BookmarkMicroservice.Api.Services.Pagination.ICheckForNextPageExistingService;

namespace BookmarkMicroservice.UnitTests
{
    public class FavoriteVacancyControllerTests
    {
        [Fact]
        public async Task GetFavoriteVacanciesAsync_ReturnsOk()
        {
            Guid employeeId = Guid.NewGuid();
            int pageNumber = 2;
            var vacancies = new List<FavoriteVacancy>
            {
                new FavoriteVacancy{Id = Guid.NewGuid()}, new FavoriteVacancy{Id = Guid.NewGuid()}, new FavoriteVacancy{Id = Guid.NewGuid()}
            };
            var mock = new Mock<IFavoriteVacancyRepository>();
            mock.Setup(x => x.GetFavoriteVacanciesByEmployeeIdAsync(employeeId, pageNumber, null))
                .ReturnsAsync(vacancies);
            var controller =
                new FavoriteVacancyController(mock.Object, new Mock<ICheckForNextPageExistingService>().Object);

            var result = await controller.GetFavoriteVacanciesAsync(employeeId, null, pageNumber);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var returnedVacancies = Assert.IsType<List<FavoriteVacancy>>(methodResult.Value);
            Assert.Equal(vacancies.Count, returnedVacancies.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task AddToFavoritesAsync_ReturnsOk()
        {
            var model = new AddVacancyDto
            {
                CompanyName = "test", Position = "test", EmployeeId = Guid.NewGuid(),
                VacancyCity = "test", VacancyId = Guid.NewGuid()
            };
            var mock = new Mock<IFavoriteVacancyRepository>();
            mock.Setup(x => x.AddToFavoritesAsync(model));
            var controller =
                new FavoriteVacancyController(mock.Object, new Mock<ICheckForNextPageExistingService>().Object);

            var result = await controller.AddToFavoritesAsync(model);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteFromFavoritesAsync_ReturnsOk()
        {
            Guid vacancyId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var mock = new Mock<IFavoriteVacancyRepository>();
            mock.Setup(x => x.DeleteFromFavoritesAsync(vacancyId, employeeId));
            var controller =
                new FavoriteVacancyController(mock.Object, new Mock<ICheckForNextPageExistingService>().Object);

            var result = await controller.DeleteFromFavoritesAsync(employeeId, vacancyId);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }
    }
}
