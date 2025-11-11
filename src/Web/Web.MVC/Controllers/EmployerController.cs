using System.Text;
using System.Text.Json;
using GeneralLibrary.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.DTOs.Employer;
using Web.MVC.Filters.Authorization_filters.Account_type_filters;
using Web.MVC.Models.ApiResponses.Employer;

namespace Web.MVC.Controllers
{
    [AccountTypeAuthorizationFilter(AccountTypeEnum.Employer)]
    public class EmployerController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;
        public EmployerController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        }


        [Authorize]
        [HttpGet]
        [Route("employer/profile/me")]
        public async Task<IActionResult> UpdateEmployer(bool? isUpdated)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            response.EnsureSuccessStatusCode();

            ViewBag.IsUpdated = isUpdated;

            var employer = await response.Content.ReadFromJsonAsync<EmployerResponse>();
            return View(new UpdateEmployerDto
            {
                Name = employer.Name, CompanyPost = employer.CompanyPost, Id = employer.Id, Surname = employer.Surname
            });
        }

        [Authorize]
        [HttpPost]
        [Route("employer/profile/me")]
        public async Task<IActionResult> UpdateEmployer(UpdateEmployerDto model)
        {
            if (ModelState.IsValid)
            {
                using HttpClient httpClient = httpClientFactory.CreateClient();
                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await httpClient.PatchAsync($"{url}/api/Employer/UpdateEmployer", jsonContent);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("UpdateEmployer", new { isUpdated = true });
            }

            return View(model);
        }
    }
}
