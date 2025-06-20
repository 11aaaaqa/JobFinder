using System.Security.Claims;
using System.Text.Json;
using AccountMicroservice.Api.DTOs;
using AccountMicroservice.Api.Kafka;
using AccountMicroservice.Api.Models;
using AccountMicroservice.Api.Services;
using Confluent.Kafka;
using GeneralLibrary.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace AccountMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService tokenService;
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;
        private readonly IKafkaProducer kafkaProducer;

        public AuthController(ITokenService tokenService, UserManager<User> userManager, IEmailService emailService, IKafkaProducer kafkaProducer)
        {
            this.tokenService = tokenService;
            this.userManager = userManager;
            this.emailService = emailService;
            this.kafkaProducer = kafkaProducer;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto model)
        {
            var user = new User
            { Email = model.Email, Id = Guid.NewGuid().ToString(), EmailConfirmed = false, AccountType = model.AccountType, UserName = model.Email };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return Conflict();

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            string confirmationLink =
                $"{model.ConfirmEmailMethodUri.Scheme}://{model.ConfirmEmailMethodUri.DomainName}/{model.ConfirmEmailMethodUri.ControllerName}/{model.ConfirmEmailMethodUri.ActionName}?token={token}&uid={user.Id}";
            await emailService.SendEmailAsync(new MailboxAddress("", user.Email), "Подтвердите свою почту",
                $"Подтвердите свою почту, перейдя по <a href=\"{confirmationLink}\">ссылке</a>.");

            await kafkaProducer.ProduceAsync("user-registered-topic", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(new { model.Name, model.Surname, model.AccountType, model.Email, AccountId = user.Id })
            });

            return Ok();
        }

        [Route("confirm-email")]
        [HttpPost]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailDto model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user is null)
                return BadRequest();

            model.Token = model.Token.Replace(" ", "+");

            var result = await userManager.ConfirmEmailAsync(user, model.Token);
            if (!result.Succeeded)
                return BadRequest();

            return Ok();
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return Unauthorized();

            var result = await userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
                return Unauthorized();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypeConstants.AccountTypeClaimName, user.AccountType)
            };

            foreach (var role in await userManager.GetRolesAsync(user))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var token = tokenService.GenerateAccessToken(claims);

            user.RefreshToken = tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(1);

            await userManager.UpdateAsync(user);

            return Ok(new AuthenticatedResponse { AccessToken = token });
        }

        [Route("AuthUserByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> AuthUserByUserNameAsync(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user is null) return BadRequest();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypeConstants.AccountTypeClaimName, user.AccountType)
            };
            foreach (var role in await userManager.GetRolesAsync(user))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = tokenService.GenerateAccessToken(claims);

            user.RefreshToken = tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(1);

            await userManager.UpdateAsync(user);

            return Ok(new AuthenticatedResponse { AccessToken = token });
        }
    }
}
