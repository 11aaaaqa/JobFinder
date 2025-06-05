using Castle.Components.DictionaryAdapter;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VacancyMicroservice.Api.Controllers;
using VacancyMicroservice.Api.DTOs;
using VacancyMicroservice.Api.Kafka.Produce;
using VacancyMicroservice.Api.Models;
using VacancyMicroservice.Api.Services;
using VacancyMicroservice.Api.Services.Pagination;

namespace VacancyMicroservice.UnitTests
{
    public class VacancyControllerTests
    {
        [Fact]
        public async Task GetVacancyByIdAsync_ReturnsOk()
        {
            Guid vacancyId = Guid.NewGuid();
            var mock = new Mock<IVacancyRepository>();
            mock.Setup(x => x.GetVacancyByIdAsync(vacancyId)).ReturnsAsync(new Vacancy
            {
                Id = vacancyId
            });
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetVacancyByIdAsync(vacancyId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var vacancy = Assert.IsType<Vacancy>(methodResult.Value);
            Assert.Equal(vacancyId, vacancy.Id);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetVacancyByIdAsync_ReturnsNotFound()
        {
            Guid vacancyId = Guid.NewGuid();
            var mock = new Mock<IVacancyRepository>();
            mock.Setup(x => x.GetVacancyByIdAsync(vacancyId)).ReturnsAsync((Vacancy?)null);
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetVacancyByIdAsync(vacancyId);

            Assert.IsType<NotFoundResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetAllVacanciesAsync_ReturnsOk()
        {
            var vacancies = new List<Vacancy>
            {
                new Vacancy(){Id = Guid.NewGuid()},
                new Vacancy(){Id = Guid.NewGuid()},
                new Vacancy(){Id = Guid.NewGuid()}
            };
            int pageNumber = 5;
            var mock = new Mock<IVacancyRepository>();
            mock.Setup(x => x.GetAllVacanciesAsync(5)).ReturnsAsync(vacancies);
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetAllVacanciesAsync(pageNumber);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var returnedVacancies = Assert.IsType<List<Vacancy>>(methodResult.Value);
            Assert.Equal(vacancies.Count, returnedVacancies.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task FindVacanciesAsync_ReturnsOk()
        {
            string searchingQuery = "test";
            int pageNumber = 5;
            var vacancies = new List<Vacancy>
            {
                new Vacancy(){Id = Guid.NewGuid(), Position = "test214"},
                new Vacancy(){Id = Guid.NewGuid(), Position = "test224"},
                new Vacancy(){Id = Guid.NewGuid(), Position = "test2124"}
            };
            var mock = new Mock<IVacancyRepository>();
            mock.Setup(x => x.SearchVacanciesAsync(searchingQuery, pageNumber)).ReturnsAsync(vacancies);
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.FindVacanciesAsync(searchingQuery, pageNumber);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var returnedVacancies = Assert.IsType<List<Vacancy>>(methodResult.Value);
            Assert.Equal(vacancies.Count, returnedVacancies.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetFilteredVacanciesAsync_ReturnsOk()
        {
            GetFilteredVacanciesDto model = new()
            {
                Position = "test"
            };
            int pageNumber = 3;
            var vacancies = new List<Vacancy>
            {
                new Vacancy(){Id = Guid.NewGuid(), Position = "test214"},
                new Vacancy(){Id = Guid.NewGuid(), Position = "test224"},
                new Vacancy(){Id = Guid.NewGuid(), Position = "test2124"}
            };
            var mock = new Mock<IVacancyRepository>();
            mock.Setup(x => x.GetFilteredVacanciesAsync(model, pageNumber))
                .ReturnsAsync(vacancies);
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetFilteredVacanciesAsync(model, pageNumber);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var returnedVacancies = Assert.IsType<List<Vacancy>>(methodResult.Value);
            Assert.Equal(vacancies.Count, returnedVacancies.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task FindFilteredVacanciesAsync_ReturnedOk()
        {
            GetFilteredVacanciesDto model = new()
            {
                Position = "test"
            };
            int pageNumber = 3;
            var vacancies = new List<Vacancy>
            {
                new Vacancy(){Id = Guid.NewGuid(), Position = "test214"},
                new Vacancy(){Id = Guid.NewGuid(), Position = "test224"},
                new Vacancy(){Id = Guid.NewGuid(), Position = "test2124"}
            };
            string searchingQuery = "test";
            var mock = new Mock<IVacancyRepository>();
            mock.Setup(x => x.SearchFilteredVacanciesAsync(model, searchingQuery, pageNumber))
                .ReturnsAsync(vacancies);
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.FindFilteredVacanciesAsync(model, searchingQuery, pageNumber);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var returnedVacancies = Assert.IsType<List<Vacancy>>(methodResult.Value);
            Assert.Equal(vacancies.Count, returnedVacancies.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetVacanciesByCompanyIdAsync_ReturnsOk()
        {
            Guid companyId = Guid.NewGuid();
            int pageNumber = 3;
            var vacancies = new List<Vacancy>
            {
                new Vacancy(){Id = Guid.NewGuid(), CompanyId = companyId},
                new Vacancy(){Id = Guid.NewGuid(), CompanyId = companyId},
                new Vacancy(){Id = Guid.NewGuid(), CompanyId = companyId}
            };
            var mock = new Mock<IVacancyRepository>();
            mock.Setup(x => x.GetVacanciesByCompanyIdAsync(companyId, pageNumber, null))
                .ReturnsAsync(vacancies);
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetVacanciesByCompanyIdAsync(companyId, pageNumber, null);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var returnedVacancies = Assert.IsType<List<Vacancy>>(methodResult.Value);
            Assert.Equal(vacancies.Count, returnedVacancies.Count);
            Assert.Equal(companyId, vacancies[0].CompanyId);
            Assert.Equal(companyId, vacancies[1].CompanyId);
            mock.VerifyAll();
        }

        [Fact]
        public async Task AddVacancyAsync_ReturnsOk()
        {
            var mock = new Mock<IVacancyRepository>();
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.AddVacancyAsync(new AddVacancyDto
            {
                Id = Guid.NewGuid(), VacancyCity = It.IsAny<string>(),
                CompanyId = Guid.NewGuid(), CompanyName = It.IsAny<string>(), Description = It.IsAny<string>(),
                EmploymentType = It.IsAny<string>(), Position = It.IsAny<string>(), RemoteWork = true
            });

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteVacancyAsync_ReturnsOk()
        {
            Guid vacancyId = Guid.NewGuid();
            var mock = new Mock<IVacancyRepository>();
            mock.Setup(x => x.DeleteVacancyAsync(vacancyId));
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.DeleteVacancyAsync(vacancyId);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UpdateVacancyAsync_ReturnsOk()
        {
            var model = new UpdateVacancyDto
            {
                Id = Guid.NewGuid(), VacancyCity = It.IsAny<string>(), Description = It.IsAny<string>(),
                EmploymentType = It.IsAny<string>(), Position = It.IsAny<string>(), RemoteWork = true
            };
            var mock = new Mock<IVacancyRepository>();
            mock.Setup(x => x.UpdateVacancyAsync(model));
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.UpdateVacancyAsync(model);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task ArchiveVacancyAsync_ReturnsOk()
        {
            Guid vacancyId = Guid.NewGuid();
            var mock = new Mock<IVacancyRepository>();
            mock.Setup(x => x.ArchiveVacancyAsync(vacancyId));
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.ArchiveVacancyAsync(vacancyId);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UnarchiveVacancyAsync_ReturnsOk()
        {
            Guid vacancyId = Guid.NewGuid();
            var mock = new Mock<IVacancyRepository>();
            mock.Setup(x => x.UnarchiveVacancyAsync(vacancyId));
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.UnarchiveVacancyAsync(vacancyId);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetArchivedVacanciesByCompanyIdAsync_ReturnsOk()
        {
            Guid companyId = Guid.NewGuid();
            int pageNumber = 3;
            var vacancies = new List<Vacancy>
            {
                new Vacancy(){Id = Guid.NewGuid(), CompanyId = companyId},
                new Vacancy(){Id = Guid.NewGuid(), CompanyId = companyId},
                new Vacancy(){Id = Guid.NewGuid(), CompanyId = companyId}
            };
            var mock = new Mock<IVacancyRepository>();
            mock.Setup(x => x.GetArchivedVacanciesByCompanyIdAsync(companyId, pageNumber, null))
                .ReturnsAsync(vacancies);
            var controller = new VacancyController(mock.Object, new Mock<IPaginationService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetArchivedVacanciesByCompanyIdAsync(companyId, pageNumber, null);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var returnedVacancies = Assert.IsType<List<Vacancy>>(methodResult.Value);
            Assert.Equal(vacancies.Count, returnedVacancies.Count);
            Assert.Equal(companyId, returnedVacancies[0].CompanyId);
            Assert.Equal(companyId, returnedVacancies[1].CompanyId);
            mock.VerifyAll();
        }
    }
}
