using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Web.MVC.Models.ApiResponses;

namespace Web.MVC.Middlewares
{
    public class JwtTokenMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;

        public JwtTokenMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.next = next;
            this.httpClientFactory = httpClientFactory;
            url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.Cookies.TryGetValue("access_token", out var accessToken);
            if (accessToken != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(accessToken);

                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    using HttpClient client = httpClientFactory.CreateClient();
                    var emailClaim = jwtToken.Claims.Single(x => x.Type == ClaimTypes.Email);

                    var getUserResponse = await client.GetAsync($"{url}/api/User/GetUserByEmail/{emailClaim.Value}");
                    if (!getUserResponse.IsSuccessStatusCode)
                    {
                        context.Response.Cookies.Delete("access_token");
                        await next(context);
                    }

                    var user = await getUserResponse.Content.ReadFromJsonAsync<IdentityUserResponse>();

                    if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
                    {
                        context.Response.Cookies.Delete("access_token");
                        await next(context);
                    }

                    using StringContent jsonContent =
                        new(JsonSerializer.Serialize(new { ExpiredAccessToken = accessToken, user.RefreshToken }), Encoding.UTF8, "application/json");
                    var refreshTokenResponse = await client.PostAsync($"{url}/api/Token/refresh", jsonContent);
                    refreshTokenResponse.EnsureSuccessStatusCode();

                    var token = await refreshTokenResponse.Content.ReadFromJsonAsync<AuthenticatedResponse>();

                    context.Response.Cookies.Append("access_token", token.AccessToken,
                        new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
                    context.Request.Headers.Add("Authorization", "Bearer " + token);
                }
                else
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + accessToken);
                }
            }
            await next(context);
        }
    }
}
