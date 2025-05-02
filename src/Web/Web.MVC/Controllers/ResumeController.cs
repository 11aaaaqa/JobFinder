using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.DTOs.Resume;
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

        [Authorize]
        [HttpGet]
        [Route("employee/profile/resumes/create")]
        public async Task<IActionResult> AddResume()
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?{User.Identity.Name}");
            response.EnsureSuccessStatusCode();

            var employee = await response.Content.ReadFromJsonAsync<EmployeeResponse>();

            return View(new AddResumeDto{Status = employee.Status});
        }

        [Authorize]
        [HttpPost]
        [Route("employee/profile/resumes/create")]
        public async Task<IActionResult> AddResume(AddResumeDto model)
        {
            if (ModelState.IsValid)
            {
                using HttpClient httpClient = httpClientFactory.CreateClient();
                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{url}/api/Resume/AddResume", jsonContent);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("GetMyResumes"); // change to redirection to created resume
            }
            return View();
        }
    }
}
