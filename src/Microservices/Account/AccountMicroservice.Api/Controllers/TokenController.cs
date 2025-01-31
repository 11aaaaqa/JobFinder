using AccountMicroservice.Api.DTOs;
using AccountMicroservice.Api.Models;
using AccountMicroservice.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AccountMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private ITokenService tokenService;
        public TokenController(UserManager<User> userManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        [HttpGet]
        [Route("revoke/{userName}")]
        public async Task<IActionResult> RevokeAsync(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user is null)
                return BadRequest();

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = new DateTime();

            await userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenDto model)
        {
            var principal = tokenService.GetPrincipalFromExpiredToken(model.ExpiredAccessToken);

            var user = await userManager.FindByNameAsync(principal.Identity.Name);

            if (user is null || model.RefreshToken != user.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return BadRequest();

            var token = tokenService.GenerateAccessToken(principal.Claims);

            user.RefreshToken = tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(1);

            await userManager.UpdateAsync(user);

            return Ok(new AuthenticatedResponse { AccessToken = token });
        }
    }
}
