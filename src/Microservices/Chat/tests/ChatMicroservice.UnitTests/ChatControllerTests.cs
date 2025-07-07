using ChatMicroservice.Api.Controllers;
using ChatMicroservice.Api.DTOs;
using ChatMicroservice.Api.Models;
using ChatMicroservice.Api.Services.Chat_services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChatMicroservice.UnitTests
{
    public class ChatControllerTests
    {
        [Fact]
        public async Task GetChatById_ReturnsOk()
        {
            Guid chatId = Guid.NewGuid();
            var mock = new Mock<IChatService>();
            mock.Setup(x => x.GetChatByIdAsync(chatId)).ReturnsAsync(new Chat
            {
                EmployeeFullName = It.IsAny<string>(), EmployeeId = Guid.NewGuid(), EmployerFullName = It.IsAny<string>(),
                EmployerId = Guid.NewGuid(), Id = chatId, LastMessageSendingTime = DateTime.UtcNow
            });
            var controller = new ChatController(mock.Object);

            var result = await controller.GetChatByChatIdAsync(chatId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var chat = Assert.IsType<Chat>(methodResult.Value);
            Assert.Equal(chatId, chat.Id);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetChatById_ReturnsNotFound()
        {
            Guid chatId = Guid.NewGuid();
            var mock = new Mock<IChatService>();
            mock.Setup(x => x.GetChatByIdAsync(chatId)).ReturnsAsync((Chat?)null);
            var controller = new ChatController(mock.Object);

            var result = await controller.GetChatByChatIdAsync(chatId);

            Assert.IsType<NotFoundResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task CreateChat_ReturnsOk()
        {
            var mock = new Mock<IChatService>();
            var controller = new ChatController(mock.Object);

            var result = await controller.CreateChatAsync(new CreateChatDto
            {
                EmployeeFullName = It.IsAny<string>(),
                EmployeeId = Guid.NewGuid(),
                EmployerFullName = It.IsAny<string>(),
                EmployerId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            });

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetChat_ReturnsOk()
        {
            Guid employeeId = Guid.NewGuid();
            Guid employerId = Guid.NewGuid();
            var mock = new Mock<IChatService>();
            mock.Setup(x => x.GetChatAsync(employeeId, employerId)).ReturnsAsync(new Chat
            {
                EmployeeFullName = It.IsAny<string>(),
                EmployeeId = employeeId,
                EmployerFullName = It.IsAny<string>(),
                EmployerId = employerId,
                Id = Guid.NewGuid(),
                LastMessageSendingTime = DateTime.UtcNow
            });
            var controller = new ChatController(mock.Object);

            var result = await controller.GetChatAsync(employeeId, employerId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var chat = Assert.IsType<Chat>(methodResult.Value);
            Assert.Equal(employeeId, chat.EmployeeId);
            Assert.Equal(employerId, chat.EmployerId);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetChat_ReturnsNotFound()
        {
            var employeeId = Guid.NewGuid();
            var employerId = Guid.NewGuid();
            var mock = new Mock<IChatService>();
            mock.Setup(x => x.GetChatAsync(employeeId, employerId)).ReturnsAsync((Chat?)null);
            var controller = new ChatController(mock.Object);

            var result = await controller.GetChatAsync(employeeId, employerId);

            Assert.IsType<NotFoundResult>(result);
            mock.VerifyAll();
        }
    }
}
