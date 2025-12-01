using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationMicroservice.Api.Controllers;
using NotificationMicroservice.Api.DTOs;
using NotificationMicroservice.Api.Models;
using NotificationMicroservice.Api.Services;

namespace NotificationMicroservice.UnitTests
{
    public class NotificationControllerTests
    {
        [Fact]
        public async Task GetNotificationByIdAsync_ReturnsNotFound()
        {
            Guid notificationId = Guid.NewGuid();
            var mock = new Mock<INotificationService>();
            mock.Setup(x => x.GetNotificationByIdAsync(notificationId)).ReturnsAsync((Notification?)null);
            var controller = new NotificationController(mock.Object);

            var result = await controller.GetNotificationByIdAsync(notificationId);

            Assert.IsType<NotFoundResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetNotificationByIdAsync_ReturnsNotification()
        {
            Guid notificationId = Guid.NewGuid();
            var mock = new Mock<INotificationService>();
            mock.Setup(x => x.GetNotificationByIdAsync(notificationId)).ReturnsAsync(new Notification { Id = notificationId });
            var controller = new NotificationController(mock.Object);

            var result = await controller.GetNotificationByIdAsync(notificationId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var returnedNotification = Assert.IsType<Notification>(methodResult.Value);
            Assert.Equal(notificationId, returnedNotification.Id);
            mock.VerifyAll();
        }

        [Fact]
        public async Task RemoveNotificationsAsync_RemovesNotifications()
        {
            var notifications = new List<Notification>
            {
                new Notification{Id = Guid.NewGuid()},
                new Notification{Id = Guid.NewGuid()}
            };
            var model = new RemoveNotificationsDto {Notifications = notifications};
            var mock = new Mock<INotificationService>();
            mock.Setup(x => x.RemoveNotificationsAsync(model.Notifications));
            var controller = new NotificationController(mock.Object);

            var result = await controller.RemoveNotificationsAsync(model);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task AddNotificationAsync_AddsNotification()
        {
            var model = new AddNotificationDto { Body = It.IsAny<string>(), Id = Guid.NewGuid(), UserEmail = It.IsAny<string>() };
            var mock = new Mock<INotificationService>();
            var controller = new NotificationController(mock.Object);

            var result = await controller.AddNotificationAsync(model);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }
    }
}
