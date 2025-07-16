using Microsoft.AspNetCore.Mvc;
using NotificationMicroservice.Api.DTOs;
using NotificationMicroservice.Api.Models;
using NotificationMicroservice.Api.Services;

namespace NotificationMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(INotificationService notificationService) : ControllerBase
    {
        [HttpGet]
        [Route("GetNotificationById/{notificationId}")]
        public async Task<IActionResult> GetNotificationByIdAsync(Guid notificationId)
        {
            var notification = await notificationService.GetNotificationByIdAsync(notificationId);
            if (notification == null)
                return NotFound();

            return Ok(notification);
        }

        [HttpGet]
        [Route("GetNotificationsByUserEmail")]
        public async Task<IActionResult> GetNotificationsByUserEmailAsync(string userEmail, int pageNumber)
            => Ok(await notificationService.GetNotificationsByUserEmailAsync(userEmail, pageNumber));

        [HttpGet]
        [Route("GetNotificationsCountByUserEmail")]
        public async Task<IActionResult> GetNotificationsCountByUserEmailAsync(string userEmail)
            => Ok(await notificationService.GetNotificationsCountByUserEmailAsync(userEmail));

        [HttpPost]
        [Route("RemoveNotifications")]
        public async Task<IActionResult> RemoveNotificationsAsync([FromBody]RemoveNotificationsDto model)
        {
            await notificationService.RemoveNotificationsAsync(model.Notifications);
            return Ok();
        }

        [HttpPost]
        [Route("AddNotification")]
        public async Task<IActionResult> AddNotificationAsync([FromBody] AddNotificationDto model)
        {
            await notificationService.AddNotificationAsync(new Notification
            {
                UserEmail = model.UserEmail, Body = model.Body,
                CreatedAt = DateTime.UtcNow, Id = model.Id
            });
            return Ok();
        }
    }
}
