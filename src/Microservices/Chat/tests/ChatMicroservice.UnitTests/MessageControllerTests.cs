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
        public async Task CreateMessageWhereSenderIsEmployer_ReturnsOk()
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
        public async Task CreateMessageWhereSenderIsEmployee_ReturnsOk()
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

        [Fact]
        public async Task MarkMessagesAsRead_ReturnsOk()
        {
            Guid chatId = Guid.NewGuid();
            var model = new MarkMessagesAsReadDto
                { CurrentAccountType = It.IsAny<AccountTypeEnum>(), Messages = new List<Message>
                {
                    new Message
                    {
                        IsRead = false, ChatId = chatId, CreatedAt = DateTime.UtcNow,
                        Id = Guid.NewGuid(), SenderId = Guid.NewGuid(), Text = It.IsAny<string>()
                    },
                    new Message
                    {
                        IsRead = false, ChatId = chatId, CreatedAt = DateTime.UtcNow,
                        Id = Guid.NewGuid(), SenderId = Guid.NewGuid(), Text = It.IsAny<string>()
                    },
                    new Message
                    {
                        IsRead = false, ChatId = chatId, CreatedAt = DateTime.UtcNow,
                        Id = Guid.NewGuid(), SenderId = Guid.NewGuid(), Text = It.IsAny<string>()
                    },
                    new Message
                    {
                        IsRead = true, ChatId = chatId, CreatedAt = DateTime.UtcNow,
                        Id = Guid.NewGuid(), SenderId = Guid.NewGuid(), Text = It.IsAny<string>()
                    },
                    new Message
                    { 
                        IsRead = false, ChatId = chatId, CreatedAt = DateTime.UtcNow, 
                        Id = Guid.NewGuid(), SenderId = Guid.NewGuid(), Text = It.IsAny<string>()
                    }
                } };
            var messageServiceMock = new Mock<IMessageService>();
            var chatServiceMock = new Mock<IChatService>();
            int markedMessagesCount = model.Messages.Count(x => x.IsRead == false);
            messageServiceMock.Setup(x => x.MarkMessagesAsReadAsync(model.Messages)).ReturnsAsync(markedMessagesCount);
            chatServiceMock.Setup(x =>
                x.DecreaseUnreadMessagesCountAsync(chatId, markedMessagesCount, model.CurrentAccountType));
            var controller = new MessageController(messageServiceMock.Object, chatServiceMock.Object);

            var result = await controller.MarkMessagesAsReadAsync(model);

            Assert.IsType<OkResult>(result);
            chatServiceMock.VerifyAll();
            messageServiceMock.VerifyAll();
        }

        [Fact]
        public async Task MarkMessageAsReadCurrentEmployeeNull_ReturnsOk()
        {
            Guid currentEmployerId = Guid.NewGuid();
            Guid? currentEmployeeId = null;
            var message = new Message
            {
                Id = Guid.NewGuid(), ChatId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, IsRead = true,
                SenderId = Guid.NewGuid(), Text = It.IsAny<string>()
            };
            var messageServiceMock = new Mock<IMessageService>();
            var chatServiceMock = new Mock<IChatService>();
            messageServiceMock.Setup(x => x.MarkMessageAsReadAsync(message.Id)).ReturnsAsync(true);
            messageServiceMock.Setup(x => x.GetMessageByIdAsync(message.Id)).ReturnsAsync(message);
            chatServiceMock.Setup(x => x.DecreaseUnreadMessagesCountAsync(message.ChatId, 1, AccountTypeEnum.Employer));
            var controller = new MessageController(messageServiceMock.Object, chatServiceMock.Object);

            var result = await controller.MarkMessageAsReadAsync(message.Id, currentEmployerId, currentEmployeeId);

            Assert.IsType<OkResult>(result);
            messageServiceMock.VerifyAll();
            chatServiceMock.VerifyAll();
        }

        [Fact]
        public async Task MarkMessageAsReadCurrentEmployerNull_ReturnsOk()
        {
            Guid currentEmployeeId = Guid.NewGuid();
            Guid? currentEmployerId = null;
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChatId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                IsRead = true,
                SenderId = Guid.NewGuid(),
                Text = It.IsAny<string>()
            };
            var messageServiceMock = new Mock<IMessageService>();
            var chatServiceMock = new Mock<IChatService>();
            messageServiceMock.Setup(x => x.MarkMessageAsReadAsync(message.Id)).ReturnsAsync(true);
            messageServiceMock.Setup(x => x.GetMessageByIdAsync(message.Id)).ReturnsAsync(message);
            chatServiceMock.Setup(x => x.DecreaseUnreadMessagesCountAsync(message.ChatId, 1, AccountTypeEnum.Employee));
            var controller = new MessageController(messageServiceMock.Object, chatServiceMock.Object);

            var result = await controller.MarkMessageAsReadAsync(message.Id, currentEmployerId, currentEmployeeId);

            Assert.IsType<OkResult>(result);
            messageServiceMock.VerifyAll();
            chatServiceMock.VerifyAll();
        }

        [Fact]
        public async Task MarkMessageAsReadBothCurrentEmployeeAndEmployerNull_ReturnsOk()
        {
            Guid? currentEmployerId = null;
            Guid? currentEmployeeId = null;
            
            var controller = new MessageController(new Mock<IMessageService>().Object, new Mock<IChatService>().Object);

            var result = await controller.MarkMessageAsReadAsync(Guid.NewGuid(), currentEmployerId, currentEmployeeId);

            Assert.IsType<BadRequestResult>(result);
        }
    }
}
