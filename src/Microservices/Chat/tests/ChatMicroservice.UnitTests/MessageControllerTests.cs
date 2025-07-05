using ChatMicroservice.Api.Controllers;
using ChatMicroservice.Api.DTOs;
using ChatMicroservice.Api.Models;
using ChatMicroservice.Api.Services.Chat_services;
using ChatMicroservice.Api.Services.Message_services;
using GeneralLibrary.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChatMicroservice.UnitTests
{
    public class MessageControllerTests
    {
        [Fact]
        public async Task GetMessageByMessageId_ReturnsOk()
        {
            Guid messageId = Guid.NewGuid();
            var mock = new Mock<IMessageService>();
            mock.Setup(x => x.GetMessageByIdAsync(messageId)).ReturnsAsync(new Message
            {
                ChatId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Id = messageId,
                Text = It.IsAny<string>(), SenderId = Guid.NewGuid()
            });
            var controller = new MessageController(mock.Object, new Mock<IChatService>().Object);

            var result = await controller.GetMessageByMessageIdAsync(messageId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var message = Assert.IsType<Message>(methodResult.Value);
            Assert.Equal(messageId, message.Id);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetMessageByMessageId_ReturnsNotFound()
        {
            Guid messageId = Guid.NewGuid();
            var mock = new Mock<IMessageService>();
            mock.Setup(x => x.GetMessageByIdAsync(messageId)).ReturnsAsync((Message?)null);
            var controller = new MessageController(mock.Object, new Mock<IChatService>().Object);

            var result = await controller.GetMessageByMessageIdAsync(messageId);

            Assert.IsType<NotFoundResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task CreateMessageWhereSenderIsEmployee_ReturnsOk()
        {
            Guid senderId = Guid.NewGuid();
            var model = new CreateMessageDto
            {
                ChatId = Guid.NewGuid(), Id = Guid.NewGuid(), SenderId = senderId, Text = It.IsAny<string>()
            };
            var chatServiceMock = new Mock<IChatService>();
            var messageServiceMock = new Mock<IMessageService>();
            chatServiceMock.Setup(x => x.GetChatByIdAsync(model.ChatId))
                .ReturnsAsync(new Chat { Id = model.ChatId, EmployeeId = senderId});
            chatServiceMock.Setup(x => x.IncreaseUnreadMessagesCountByOneAsync(model.ChatId, AccountTypeEnum.Employer));
            chatServiceMock.Setup(x => x.UpdateLastMessageSendingTimeAsync(model.ChatId));
            var controller = new MessageController(messageServiceMock.Object, chatServiceMock.Object);

            var result = await controller.CreateMessageAsync(model);

            Assert.IsType<OkResult>(result);
            chatServiceMock.VerifyAll();
            messageServiceMock.VerifyAll();
        }

        [Fact]
        public async Task CreateMessageWhereSenderIsEmployer_ReturnsOk()
        {
            Guid senderId = Guid.NewGuid();
            var model = new CreateMessageDto
            {
                ChatId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                SenderId = senderId,
                Text = It.IsAny<string>()
            };
            var chatServiceMock = new Mock<IChatService>();
            var messageServiceMock = new Mock<IMessageService>();
            chatServiceMock.Setup(x => x.GetChatByIdAsync(model.ChatId))
                .ReturnsAsync(new Chat { Id = model.ChatId, EmployerId = senderId });
            chatServiceMock.Setup(x => x.IncreaseUnreadMessagesCountByOneAsync(model.ChatId, AccountTypeEnum.Employee));
            chatServiceMock.Setup(x => x.UpdateLastMessageSendingTimeAsync(model.ChatId));
            var controller = new MessageController(messageServiceMock.Object, chatServiceMock.Object);

            var result = await controller.CreateMessageAsync(model);

            Assert.IsType<OkResult>(result);
            chatServiceMock.VerifyAll();
            messageServiceMock.VerifyAll();
        }
    }
}
