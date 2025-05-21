using EmployerMicroservice.Api.Controllers;
using EmployerMicroservice.Api.DTOs;
using EmployerMicroservice.Api.Models;
using EmployerMicroservice.Api.Services;
using EmployerMicroservice.Api.Services.Company_permissions_services;
using EmployerMicroservice.Api.Services.Pagination;
using EmployerMicroservice.Api.Services.Searching_services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EmployerMicroservice.UnitTests
{
    public class EmployerControllerTests
    {
        [Fact]
        public async Task GetEmployerByIdAsync_ReturnsOk()
        {
            Guid employerId = Guid.NewGuid();
            var mock = new Mock<IEmployerRepository>();
            mock.Setup(x => x.GetEmployerByIdAsync(employerId))
                .ReturnsAsync(new Employer { Id = employerId });
            var controller = new EmployerController(mock.Object, new Mock<IPaginationService>().Object,
                new Mock<ISearchingService>().Object, new Mock<IEmployerPermissionsService>().Object);

            var result = await controller.GetEmployerByIdAsync(employerId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var returnedEmployer = Assert.IsType<Employer>(methodResult.Value);
            Assert.Equal(employerId, returnedEmployer.Id);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetEmployerByIdAsync_ReturnsBadRequest()
        {
            Guid employerId = Guid.NewGuid();
            var mock = new Mock<IEmployerRepository>();
            mock.Setup(x => x.GetEmployerByIdAsync(employerId))
                .ReturnsAsync((Employer?) null);
            var controller = new EmployerController(mock.Object, new Mock<IPaginationService>().Object,
                new Mock<ISearchingService>().Object, new Mock<IEmployerPermissionsService>().Object);

            var result = await controller.GetEmployerByIdAsync(employerId);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetEmployerByEmailAsync_ReturnsOk()
        {
            string email = It.IsAny<string>();
            var mock = new Mock<IEmployerRepository>();
            mock.Setup(x => x.GetEmployerByEmailAsync(email))
                .ReturnsAsync(new Employer { Email = email});
            var controller = new EmployerController(mock.Object, new Mock<IPaginationService>().Object,
                new Mock<ISearchingService>().Object, new Mock<IEmployerPermissionsService>().Object);

            var result = await controller.GetEmployerByEmailAsync(email);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var returnedEmployer = Assert.IsType<Employer>(methodResult.Value);
            Assert.Equal(email, returnedEmployer.Email);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetEmployerByEmailAsync_ReturnsBadRequest()
        {
            string email = It.IsAny<string>();
            var mock = new Mock<IEmployerRepository>();
            mock.Setup(x => x.GetEmployerByEmailAsync(email))
                .ReturnsAsync((Employer?)null);
            var controller = new EmployerController(mock.Object, new Mock<IPaginationService>().Object,
                new Mock<ISearchingService>().Object, new Mock<IEmployerPermissionsService>().Object);

            var result = await controller.GetEmployerByEmailAsync(email);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UpdateEmployerAsync_ReturnsOk()
        {
            var model = new UpdateEmployerDto {Id = Guid.NewGuid(), Name = "test", Surname = "test"};
            var mock = new Mock<IEmployerRepository>();
            mock.Setup(x => x.UpdateEmployerAsync(model)).ReturnsAsync(true);
            var controller = new EmployerController(mock.Object, new Mock<IPaginationService>().Object,
                new Mock<ISearchingService>().Object, new Mock<IEmployerPermissionsService>().Object);

            var result = await controller.UpdateEmployerAsync(model);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UpdateEmployerAsync_ReturnsBadRequest()
        {
            var model = new UpdateEmployerDto { Id = Guid.NewGuid(), Name = "test", Surname = "test" };
            var mock = new Mock<IEmployerRepository>();
            mock.Setup(x => x.UpdateEmployerAsync(model)).ReturnsAsync(false);
            var controller = new EmployerController(mock.Object, new Mock<IPaginationService>().Object,
                new Mock<ISearchingService>().Object, new Mock<IEmployerPermissionsService>().Object);

            var result = await controller.UpdateEmployerAsync(model);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }
    }
}
