using Microsoft.AspNetCore.Mvc;
using Moq;
using ResumeMicroservice.Api.Controllers;
using ResumeMicroservice.Api.DTOs;
using ResumeMicroservice.Api.Kafka.Producing;
using ResumeMicroservice.Api.Models;
using ResumeMicroservice.Api.Services.Pagination;
using ResumeMicroservice.Api.Services.Repositories;

namespace ResumeMicroservice.UnitTests
{
    public class ResumeControllerTests
    {
        [Fact]
        public async Task GetResumeByIdAsync_ReturnsOk()
        {
            Guid resumeId = Guid.NewGuid();
            var mock = new Mock<IResumeRepository>();
            mock.Setup(x => x.GetResumeByIdAsync(resumeId))
                .ReturnsAsync(new Resume { Id = resumeId });
            var controller = new ResumeController(mock.Object, new Mock<ICheckForNextPageExistingService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetResumeByIdAsync(resumeId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var returnedResume = Assert.IsType<Resume>(methodResult.Value);
            Assert.Equal(resumeId, returnedResume.Id);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetResumeByIdAsync_ReturnsBadRequest()
        {
            Guid resumeId = Guid.NewGuid();
            var mock = new Mock<IResumeRepository>();
            mock.Setup(x => x.GetResumeByIdAsync(resumeId))
                .ReturnsAsync((Resume?) null);
            var controller = new ResumeController(mock.Object, new Mock<ICheckForNextPageExistingService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetResumeByIdAsync(resumeId);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetAllResumesAsync_ReturnsOk()
        {
            int pageNumber = 3;
            var mock = new Mock<IResumeRepository>();
            var resumesList = new List<Resume>
            {
                new Resume{Id = Guid.NewGuid()}, new Resume{Id = Guid.NewGuid()}
            };
            mock.Setup(x => x.GetAllResumesAsync(null, pageNumber))
                .ReturnsAsync(resumesList);
            var controller = new ResumeController(mock.Object, new Mock<ICheckForNextPageExistingService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetAllResumesAsync(null, pageNumber);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var returnedResumes = Assert.IsType<List<Resume>>(methodResult.Value);
            Assert.Equal(resumesList.Count, returnedResumes.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetResumesWithActiveStatusAsync_ReturnsOk()
        {
            int pageNumber = 3;
            var mock = new Mock<IResumeRepository>();
            var resumesList = new List<Resume>
            {
                new Resume{Id = Guid.NewGuid()}, new Resume{Id = Guid.NewGuid()}
            };
            mock.Setup(x => x.GetResumesWithActiveStatusAsync(null, pageNumber))
                .ReturnsAsync(resumesList);
            var controller = new ResumeController(mock.Object, new Mock<ICheckForNextPageExistingService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetResumesWithActiveStatusAsync(null, pageNumber);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var returnedResumes = Assert.IsType<List<Resume>>(methodResult.Value);
            Assert.Equal(resumesList.Count, returnedResumes.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetFilteredResumesAsync_ReturnsOk()
        {
            var model = new ResumeFilterModel { ResumeTitle = "Test" };
            int pageNumber = 3;
            var resumesList = new List<Resume>
            {
                new Resume{Id = Guid.NewGuid(), ResumeTitle = "test214"}, new Resume{Id = Guid.NewGuid(), ResumeTitle = "4512tests"}
            };
            var mock = new Mock<IResumeRepository>();
            mock.Setup(x => x.GetFilteredResumesAsync(model, null, pageNumber))
                .ReturnsAsync(resumesList);
            var controller = new ResumeController(mock.Object, new Mock<ICheckForNextPageExistingService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetFilteredResumesAsync(model, null, pageNumber);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var returnedResumes = Assert.IsType<List<Resume>>(methodResult.Value);
            Assert.Equal(resumesList.Count, returnedResumes.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetResumesByEmployeeIdAsync_ReturnsOk()
        {
            Guid employeeId = Guid.NewGuid();
            var resumesList = new List<Resume>
            {
                new Resume{Id = Guid.NewGuid(), EmployeeId = employeeId}, new Resume{Id = Guid.NewGuid(), EmployeeId = employeeId}
            };
            var mock = new Mock<IResumeRepository>();
            mock.Setup(x => x.GetResumesByEmployeeIdAsync(employeeId)).ReturnsAsync(resumesList);
            var controller = new ResumeController(mock.Object, new Mock<ICheckForNextPageExistingService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.GetResumesByEmployeeIdAsync(employeeId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var returnedResumes = Assert.IsType<List<Resume>>(methodResult.Value);
            Assert.Equal(resumesList.Count, returnedResumes.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteResumeAsync_ReturnsOk()
        {
            Guid resumeId = Guid.NewGuid();
            var mock = new Mock<IResumeRepository>();
            mock.Setup(x => x.DeleteResumeAsync(resumeId));
            var controller = new ResumeController(mock.Object, new Mock<ICheckForNextPageExistingService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.DeleteResumeAsync(resumeId);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task AddResumeAsync_ReturnsOk()
        {
            var mock = new Mock<IResumeRepository>();
            var controller = new ResumeController(mock.Object, new Mock<ICheckForNextPageExistingService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.AddResumeAsync(new AddResumeDto
            {
                Id = Guid.NewGuid(), Name = "test", EmployeeId = Guid.NewGuid(), Status = "test",
                ReadyToMove = false, ResumeTitle = "test", Surname = "test"
            });

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UpdateResumeAsync_ReturnsOk()
        {
            var mock = new Mock<IResumeRepository>();
            var controller = new ResumeController(mock.Object, new Mock<ICheckForNextPageExistingService>().Object, new Mock<IKafkaProducer>().Object);

            var result = await controller.UpdateResumeAsync(new UpdateResumeControllerDto()
            {
                Id = Guid.NewGuid(),
                Name = "test",
                ReadyToMove = false,
                ResumeTitle = "test",
                Surname = "test"
            });

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }
    }
}
