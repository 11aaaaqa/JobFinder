using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.DTOs.Auth;
using Web.MVC.Models.ApiResponses;
using Uri = Web.MVC.Models.Uri;

namespace Web.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;
        private readonly IConfiguration configuration;
        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        }

        [HttpGet]
        [Route("auth/login")]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginDto{ReturnUrl = returnUrl});
        }

        [HttpPost]
        [Route("auth/login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                using HttpClient client = httpClientFactory.CreateClient();
                var userResponse = await client.GetAsync($"{url}/api/User/GetUserByEmail/{model.Email}");
                if (!userResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Пользователя с таким адресом эл. почты не существует");
                    return View(model);
                }
                var user = await userResponse.Content.ReadFromJsonAsync<IdentityUserResponse>();
                if (!user.EmailConfirmed)
                {
                    var checkUserPasswordResponse =
                        await client.GetAsync($"{url}/api/User/CheckUserPassword/{user.Email}?password={model.Password}");
                    bool isPasswordCorrect = await checkUserPasswordResponse.Content.ReadFromJsonAsync<bool>();
                    if (!isPasswordCorrect)
                    {
                        ModelState.AddModelError(string.Empty, "Неправильный пароль");
                        return View(model);
                    }

                    var sendVerificationEmailResponse =
                        await client.GetAsync($"{url}/api/User/SendVerificationEmail/{user.Email}?scheme={configuration["Url:MvcProj:Scheme"]}&domainName={configuration["Url:MvcProj:Domain"]}&controllerName=auth&actionName=confirm-email");
                    sendVerificationEmailResponse.EnsureSuccessStatusCode();
                    return View("LoginAccountEmailVerificationWasSent");
                }

                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{url}/api/Auth/login", jsonContent);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, "Неправильный пароль");
                    return View(model);
                }
                if (!response.IsSuccessStatusCode) return View("ActionError");

                var responseToken = await response.Content.ReadFromJsonAsync<AuthenticatedResponse>();
                Response.Cookies.Append("access_token", responseToken.AccessToken, new CookieOptions
                {
                    SameSite = SameSiteMode.Strict,
                    Secure = true,
                    HttpOnly = true
                });
                if (string.IsNullOrEmpty(model.ReturnUrl) || model.ReturnUrl.Contains("auth/register"))
                    return RedirectToAction("Index", "Home");
                return LocalRedirect(model.ReturnUrl);
            }
            return View(model);
        }

        [HttpGet]
        [Route("auth/register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("auth/register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                using HttpClient client = httpClientFactory.CreateClient();
                model.ConfirmEmailMethodUri = new Uri
                {
                    Scheme = configuration["Url:MvcProj:Scheme"],
                    DomainName = configuration["Url:MvcProj:Domain"],
                    ControllerName = "auth",
                    ActionName = "confirm-email"
                };
                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8,"application/json");

                var response = await client.PostAsync(
                    $"{url}/api/Auth/register", jsonContent);
                if (response.IsSuccessStatusCode)
                    return View("NotificationAboutConfirmingEmail");

                ModelState.AddModelError(string.Empty,
                    response.StatusCode == HttpStatusCode.Conflict
                        ? "Такой пользователь уже существует"
                        : "Что-то пошло не так, попробуйте еще раз");
            }
            return View(model);
        }

        [Route("auth/confirm-email")] //sensible route, changing will affect register method working
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string uid)
        {
            using HttpClient client = httpClientFactory.CreateClient();
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new { UserId = uid, Token = token}), Encoding.UTF8,"application/json");

            var response = await client.PostAsync($"{url}/api/Auth/confirm-email", jsonContent);
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var getUserResponse = await client.GetAsync($"{url}/api/User/GetUserByUserId/{uid}");
            if (!getUserResponse.IsSuccessStatusCode) return View("ActionError");
            var user = await getUserResponse.Content.ReadFromJsonAsync<IdentityUserResponse>();

            var authUserResponse = await client.GetAsync($"{url}/api/Auth/AuthUserByUserName/{user.UserName}");
            if (!authUserResponse.IsSuccessStatusCode) return View("ActionError");

            var accessToken = await authUserResponse.Content.ReadFromJsonAsync<AuthenticatedResponse>();
            Response.Cookies.Append("access_token", accessToken.AccessToken, new CookieOptions
            {
                SameSite = SameSiteMode.Strict, Secure = true, HttpOnly = true
            });

            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("auth/logout")]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            var userName = User.Identity.Name;
            using HttpClient client = httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{url}/api/Token/revoke/{userName}");
            response.EnsureSuccessStatusCode();

            Response.Cookies.Delete("access_token");

            return LocalRedirect(returnUrl);
        }
    }
}
