using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.Resume;

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
            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            employeeResponse.EnsureSuccessStatusCode();

            var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

            var resumesResponse = await httpClient.GetAsync($"{url}/api/Resume/GetResumesByEmployeeId/{employee.Id}");
            resumesResponse.EnsureSuccessStatusCode();

            var resumes = await resumesResponse.Content.ReadFromJsonAsync<List<ResumeResponse>>();

            ViewBag.Employee = employee;

            return View(resumes);
        }
    }
}
