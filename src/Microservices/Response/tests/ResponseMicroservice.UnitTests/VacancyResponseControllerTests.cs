using Microsoft.AspNetCore.Mvc;
using Moq;
using ResponseMicroservice.Api.Constants;
using ResponseMicroservice.Api.Controllers;
using ResponseMicroservice.Api.DTOs;
using ResponseMicroservice.Api.Models;
using ResponseMicroservice.Api.Services.Interview_invitation_services;
using ResponseMicroservice.Api.Services.Pagination;
using ResponseMicroservice.Api.Services.Vacancy_response_services;

namespace ResponseMicroservice.UnitTests
{
    public class VacancyResponseControllerTests
    {
        [Fact]
        public async Task AddVacancyResponseAsync_ReturnsOk()
        {
            var mock = new Mock<IVacancyResponseService>();
            var controller = new VacancyResponseController(mock.Object,
                new Mock<ICheckForNextPageExistingService>().Object,
                new Mock<IInterviewInvitationService>().Object);

            var result = await controller.AddVacancyResponseAsync(new AddVacancyResponseDto
            {
                EmployeeId = Guid.NewGuid(), VacancyCity = It.IsAny<string>(), VacancyId = Guid.NewGuid(),
                EmployeeSurname = It.IsAny<string>(), RespondedEmployeeResumeId = Guid.NewGuid(), EmployeeName = It.IsAny<string>(),
                EmployeeWorkingExperience = TimeSpan.Zero, VacancyCompanyId = Guid.NewGuid(), VacancyPosition = It.IsAny<string>()
            });

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task RejectVacancyResponseAsync_ReturnsOk()
        {
            Guid vacancyResponseId = Guid.NewGuid();
            var mock = new Mock<IVacancyResponseService>();
            mock.Setup(x => x.SetVacancyResponseStatusAsync(vacancyResponseId, VacancyResponseStatusConstants.Rejected));
            var controller = new VacancyResponseController(mock.Object,
                new Mock<ICheckForNextPageExistingService>().Object,
                new Mock<IInterviewInvitationService>().Object);

            var result = await controller.RejectVacancyResponseAsync(vacancyResponseId);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task AcceptVacancyResponseAsync_ReturnsBadRequest()
        {
            Guid vacancyResponseId = Guid.NewGuid();
            var mock = new Mock<IVacancyResponseService>();
            mock.Setup(x => x.GetVacancyResponseByIdAsync(vacancyResponseId)).ReturnsAsync((VacancyResponse?)null);
            var controller = new VacancyResponseController(mock.Object,
                new Mock<ICheckForNextPageExistingService>().Object,
                new Mock<IInterviewInvitationService>().Object);

            var result = await controller.AcceptVacancyResponseAsync(vacancyResponseId);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task AcceptVacancyResponseAsync_ReturnsOk()
        {
            Guid vacancyResponseId = Guid.NewGuid();
            var vacancyResponse = new VacancyResponse
            {
                Id = vacancyResponseId, EmployeeId = Guid.NewGuid(), VacancyCity = It.IsAny<string>(), VacancyId = Guid.NewGuid(),
                EmployeeName = It.IsAny<string>(), EmployeeSurname = It.IsAny<string>(), VacancyPosition = It.IsAny<string>(),
                ResponseDate = DateTime.UtcNow, RespondedEmployeeResumeId = Guid.NewGuid(), ResponseStatus = It.IsAny<string>(),
                EmployeeWorkingExperience = TimeSpan.Zero, VacancyCompanyId = Guid.NewGuid(), EmployeeCity = It.IsAny<string>(),
                EmployeeDateOfBirth = DateOnly.MaxValue, VacancySalaryFrom = It.IsAny<int>(), VacancyWorkExperience = It.IsAny<string>(),
                VacancySalaryTo = It.IsAny<int>()
            };
            var vacancyResponseMock = new Mock<IVacancyResponseService>();
            var interviewInvitationMock = new Mock<IInterviewInvitationService>();
            vacancyResponseMock.Setup(x => x.GetVacancyResponseByIdAsync(vacancyResponseId)).ReturnsAsync(vacancyResponse);
            vacancyResponseMock.Setup(x =>
                x.SetVacancyResponseStatusAsync(vacancyResponseId, VacancyResponseStatusConstants.Accepted));
            var controller = new VacancyResponseController(vacancyResponseMock.Object,
                new Mock<ICheckForNextPageExistingService>().Object,
                interviewInvitationMock.Object);

            var result = await controller.AcceptVacancyResponseAsync(vacancyResponseId);

            Assert.IsType<OkResult>(result);
            vacancyResponseMock.VerifyAll();
            interviewInvitationMock.VerifyAll();
        }
    }
}
