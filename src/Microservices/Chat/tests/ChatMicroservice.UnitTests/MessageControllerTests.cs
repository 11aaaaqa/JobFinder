using ChatMicroservice.Api.Controllers;
using ChatMicroservice.Api.DTOs;
using ChatMicroservice.Api.Models;
using ChatMicroservice.Api.Services.Chat_services;
using ChatMicroservice.Api.Services.Message_services;
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
        public async Task CreateMessage_ReturnsOk()
        {
            var model = new CreateMessageDto{ChatId = Guid.NewGuid(), Text = It.IsAny<string>(), SenderId = Guid.NewGuid()};
            var chatMock = new Mock<IChatService>();
            var messageMock = new Mock<IMessageService>();
            chatMock.Setup(x => x.UpdateLastMessageSendingTimeAsync(model.ChatId));
            var controller = new MessageController(messageMock.Object, chatMock.Object);

            var result = await controller.CreateMessageAsync(model);

            Assert.IsType<OkResult>(result);
            messageMock.VerifyAll();
            chatMock.Verify(x => x.UpdateLastMessageSendingTimeAsync(model.ChatId));
        }
    }
}
