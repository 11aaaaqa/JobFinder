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

            var response = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            response.EnsureSuccessStatusCode();

            var employee = await response.Content.ReadFromJsonAsync<EmployeeResponse>();
            return View(new AddResumeDto
            {
                Id = Guid.NewGuid(), Status = employee.Status, EmployeeId = employee.Id, Name = employee.Name,
                Surname = employee.Surname, Email = employee.Email, DateOfBirth = employee.DateOfBirth, City = employee.City,
                PhoneNumber = employee.PhoneNumber, Patronymic = employee.Patronymic, Gender = employee.Gender
            });
        }

        [Authorize]
        [HttpPost]
        [Route("employee/profile/resumes/create")]
        public async Task<IActionResult> AddResume(AddResumeDto model)
        {
            if (model.Educations is not null && model.Educations.Count == 0)
                model.Educations = null;
            if (model.EmployeeExperience is not null && model.EmployeeExperience.Count == 0)
                model.EmployeeExperience = null;
            if (model.ForeignLanguages is not null && model.ForeignLanguages.Count == 0)
                model.ForeignLanguages = null;
            if (ModelState.IsValid)
            {
                if (model.EmployeeExperience is not null)
                {
                    foreach (var experience in model.EmployeeExperience)
                    {
                        DateTime workingFrom = DateTime.ParseExact(experience.WorkingFrom + "-01", "yyyy-MM-dd", null);
                        DateTime workingUntil = DateTime.ParseExact(experience.WorkingUntil + "-01", "yyyy-MM-dd", null);
                        experience.WorkingDuration = workingUntil - workingFrom;
                    }
                }
                using HttpClient httpClient = httpClientFactory.CreateClient();
                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{url}/api/Resume/AddResume", jsonContent);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("GetResume", new { resumeId = model.Id});
            }
            return View();
        }

        [HttpGet]
        [Route("resume/{resumeId}")]
        public async Task<IActionResult> GetResume(Guid resumeId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"{url}/api/Resume/GetResumeById/{resumeId}");
            response.EnsureSuccessStatusCode();

            var resume = await response.Content.ReadFromJsonAsync<ResumeResponse>();

            return View(resume);
        }
    }
}
