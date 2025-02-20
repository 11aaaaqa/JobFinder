using Microsoft.AspNetCore.Mvc;
using Moq;
using CompanyMicroservice.Api.Controllers;
using CompanyMicroservice.Api.DTOs;
using CompanyMicroservice.Api.Kafka.Producer;
using CompanyMicroservice.Api.Models;
using CompanyMicroservice.Api.Services;

namespace CompanyMicroservice.UnitTests
{
    public class CompanyControllerTests
    {
        [Fact]
        public async Task GetCompanyByCompanyIdAsync_ReturnsBadRequest()
        {
            var companyId = Guid.NewGuid();
            var mock = new Mock<ICompanyRepository>();
            mock.Setup(x => x.GetCompanyByIdAsync(companyId)).ReturnsAsync((Company?)null);
            var controller = new CompanyController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetCompanyByCompanyIdAsync(companyId);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetCompanyByCompanyIdAsync_ReturnsOk()
        {
            var companyId = Guid.NewGuid();
            var company = new Company
            {
                Id = companyId,
                CompanyName = It.IsAny<string>(),
                CompanyDescription = It.IsAny<string>(),
                CompanyColleaguesCount = It.IsAny<string>(),
                FounderEmployerId = Guid.NewGuid()
            };
            var mock = new Mock<ICompanyRepository>();
            mock.Setup(x => x.GetCompanyByIdAsync(companyId)).ReturnsAsync(company);
            var controller = new CompanyController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetCompanyByCompanyIdAsync(companyId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var addedCompany = Assert.IsType<Company>(methodResult.Value);
            Assert.Equal(company, addedCompany);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetCompanyByCompanyNameAsync_ReturnsBadRequest()
        {
            var companyName = "TestCompanyName";
            var mock = new Mock<ICompanyRepository>();
            mock.Setup(x => x.GetCompanyByCompanyNameAsync(companyName)).ReturnsAsync((Company?)null);
            var controller = new CompanyController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetCompanyByCompanyNameAsync(companyName);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetCompanyByCompanyNameAsync_ReturnsOk()
        {
            var companyName = "TestCompanyName";
            var company = new Company
            {
                Id = Guid.NewGuid(),
                CompanyName = companyName,
                CompanyDescription = It.IsAny<string>(),
                CompanyColleaguesCount = It.IsAny<string>(),
                FounderEmployerId = Guid.NewGuid()
            };
            var mock = new Mock<ICompanyRepository>();
            mock.Setup(x => x.GetCompanyByCompanyNameAsync(companyName)).ReturnsAsync(company);
            var controller = new CompanyController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetCompanyByCompanyNameAsync(companyName);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var addedCompany = Assert.IsType<Company>(methodResult.Value);
            Assert.Equal(company, addedCompany);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UpdateCompanyAsync_ReturnsBadRequest()
        {
            var nonExistentId = Guid.NewGuid();
            UpdateCompanyDto model = new()
            {
                Id = nonExistentId,
                CompanyColleaguesCount = It.IsAny<string>(),
                CompanyDescription = It.IsAny<string>(),
                CompanyName = It.IsAny<string>()
            };
            var mock = new Mock<ICompanyRepository>();
            mock.Setup(x => x.UpdateCompanyAsync(model)).ReturnsAsync(false);
            var controller = new CompanyController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.UpdateCompanyAsync(model);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UpdateCompanyAsync_ReturnsOk()
        {
            var existingId = Guid.NewGuid();
            UpdateCompanyDto model = new()
            {
                Id = existingId,
                CompanyColleaguesCount = It.IsAny<string>(),
                CompanyDescription = It.IsAny<string>(),
                CompanyName = It.IsAny<string>()
            };
            var mock = new Mock<ICompanyRepository>();
            mock.Setup(x => x.UpdateCompanyAsync(model)).ReturnsAsync(true);
            var controller = new CompanyController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.UpdateCompanyAsync(model);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteCompanyAsync_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var mock = new Mock<ICompanyRepository>();
            mock.Setup(x => x.DeleteCompanyAsync(id));
            var controller = new CompanyController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.DeleteCompanyAsync(new DeleteCompanyDto { CompanyId = id });

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task AddCompanyAsync_ReturnsOk()
        {
            var model = new AddCompanyDto
            {
                CompanyName = It.IsAny<string>(),
                CompanyDescription = It.IsAny<string>(),
                CompanyColleaguesCount = It.IsAny<string>(),
                FounderEmployerId = Guid.NewGuid()
            };
            var mock = new Mock<ICompanyRepository>();
            var controller = new CompanyController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.AddCompanyAsync(model);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }
    }
}
