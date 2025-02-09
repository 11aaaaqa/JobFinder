using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Models.ApiResponses;

namespace Web.MVC.Controllers
{
    public class ResumeController : Controller
    {
        private readonly string url;
        private readonly IHttpClientFactory httpClientFactory;
        public ResumeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        }

        [Authorize]
        [HttpGet]
        [Route("employee/profile/resumes")]
        public async Task<IActionResult> GetMyResumes()
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            response.EnsureSuccessStatusCode();

            var employee = await response.Content.ReadFromJsonAsync<EmployeeResponse>();

            return View(employee);
        }
    }
}
