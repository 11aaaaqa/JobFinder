using EmployerMicroservice.Api.Controllers;
using EmployerMicroservice.Api.Models;
using EmployerMicroservice.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Guid = System.Guid;

namespace EmployerMicroservice.UnitTests
{
    public class CompanyControllerUnitTests
    {
        [Fact]
        public async Task GetCompanyByCompanyIdAsync_ReturnsBadRequest()
        {
            var companyId = Guid.NewGuid();
            var mock = new Mock<ICompanyRepository>();
            mock.Setup(x => x.GetCompanyByIdAsync(companyId)).ReturnsAsync((Company?)null);
            var controller = new CompanyController(mock.Object);

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
                Id = companyId, CompanyName = It.IsAny<string>(), CompanyDescription = It.IsAny<string>(),
                CompanyColleaguesCount = It.IsAny<uint>(), FounderEmployerId = Guid.NewGuid()
            };
            var mock = new Mock<ICompanyRepository>();
            mock.Setup(x => x.GetCompanyByIdAsync(companyId)).ReturnsAsync(company);
            var controller = new CompanyController(mock.Object);

            var result = await controller.GetCompanyByCompanyIdAsync(companyId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var addedCompany = Assert.IsType<Company>(methodResult.Value);
            Assert.Equal(companyId, addedCompany.Id);
            mock.VerifyAll();
        }
    }
}
