using EmployeeMicroservice.Api.Controllers;
using EmployeeMicroservice.Api.DTOs;
using EmployeeMicroservice.Api.Kafka.Kafka_producer;
using EmployeeMicroservice.Api.Models;
using EmployeeMicroservice.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EmployeeMicroservice.UnitTests
{
    public class EmployeeControllerTests
    {
        [Fact]
        public async Task GetEmployeeByIdAsync_ReturnsOk()
        {
            Guid employeeId = Guid.NewGuid();
            var mock = new Mock<IEmployeeRepository>();
            mock.Setup(x => x.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync(new Employee { Id = employeeId });
            var controller = new EmployeeController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetEmployeeByIdAsync(employeeId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var returnedEmployee = Assert.IsType<Employee>(methodResult.Value);
            Assert.Equal(employeeId, returnedEmployee.Id);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ReturnsBadRequest()
        {
            Guid employeeId = Guid.NewGuid();
            var mock = new Mock<IEmployeeRepository>();
            mock.Setup(x => x.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync((Employee?) null);
            var controller = new EmployeeController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetEmployeeByIdAsync(employeeId);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetEmployeeByEmailAsync_ReturnsOk()
        {
            string email = It.IsAny<string>();
            var mock = new Mock<IEmployeeRepository>();
            mock.Setup(x => x.GetEmployeeByEmailAsync(email))
                .ReturnsAsync(new Employee { Email = email });
            var controller = new EmployeeController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetEmployeeByEmailAsync(email);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var returnedEmployee = Assert.IsType<Employee>(methodResult.Value);
            Assert.Equal(email, returnedEmployee.Email);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetEmployeeByEmailAsync_ReturnsBadRequest()
        {
            string email = It.IsAny<string>();
            var mock = new Mock<IEmployeeRepository>();
            mock.Setup(x => x.GetEmployeeByEmailAsync(email))
                .ReturnsAsync((Employee?)null);
            var controller = new EmployeeController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetEmployeeByEmailAsync(email);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UpdateEmployeeAsync_ReturnsOk()
        {
            var model = new UpdateEmployeeDto { Id = Guid.NewGuid(), Name = "Name", Surname = "Surname"};
            var mock = new Mock<IEmployeeRepository>();
            mock.Setup(x => x.GetEmployeeByIdAsync(model.Id)).ReturnsAsync(new Employee
            {
                Id = model.Id, Name = model.Name, Surname = model.Surname
            });
            mock.Setup(x => x.UpdateEmployeeAsync(model));
            var controller = new EmployeeController(mock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.UpdateEmployeeAsync(model);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UpdateEmployeeStatusAsync_ReturnsOk()
        {
            var model = new UpdateEmployeeStatusDto { EmployeeId = Guid.NewGuid(), Status = "status" };
            var employeeMock = new Mock<IEmployeeRepository>();
            employeeMock.Setup(x => x.GetEmployeeByIdAsync(model.EmployeeId)).ReturnsAsync(new Employee
            {
                Id = model.EmployeeId
            });
            employeeMock.Setup(x => x.UpdateEmployeeStatusAsync(model.EmployeeId, model.Status));
            var controller = new EmployeeController(employeeMock.Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.UpdateEmployeeStatusAsync(model);

            Assert.IsType<OkResult>(result);
            employeeMock.VerifyAll();
        }
    }
}
