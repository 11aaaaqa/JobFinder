using AccountMicroservice.Api.Models;
using AccountMicroservice.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace AccountMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserManager<User> userManager, IEmailService emailService) : ControllerBase
    {
        [Route("GetUserByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetUserByUserIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return BadRequest();

            return Ok(user);
        }

        [Route("GetUserByEmail/{email}")]
        [HttpGet]
        public async Task<IActionResult> GetUserByEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) return BadRequest();

            return Ok(user);
        }

        [Route("SendVerificationEmail/{email}")]
        [HttpGet]
        public async Task<IActionResult> SendVerificationEmailAsync(string email, string scheme, string domainName, string controllerName, string actionName)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) return NotFound();
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            string confirmationLink =
                $"{scheme}://{domainName}/{controllerName}/{actionName}?token={token}&uid={user.Id}";
            await emailService.SendEmailAsync(new MailboxAddress("", user.Email), "Подтвердите свою почту",
                $"Подтвердите свою почту, перейдя по <a href=\"{confirmationLink}\">ссылке</a>.");
            return Ok();
        }

        [Route("CheckUserPassword/{email}")]
        [HttpGet]
        public async Task<IActionResult> CheckUserPasswordAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) return NotFound();
            var result = await userManager.CheckPasswordAsync(user, password);
            return Ok(result);
        }
    }
}
