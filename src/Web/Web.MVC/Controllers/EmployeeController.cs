using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.DTOs.Employee;
using Web.MVC.Models.ApiResponses;

namespace Web.MVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;
        public EmployeeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        }

        [Authorize]
        [Route("profile/me")]
        [HttpGet]
        public async Task<IActionResult> UpdateEmployee(bool? isUpdated)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            response.EnsureSuccessStatusCode();

            ViewBag.IsUpdated = isUpdated;

            var employee = await response.Content.ReadFromJsonAsync<EmployeeResponse>();
            return View(new UpdateEmployeeDto
            {
                Name = employee.Name, City = employee.City, DateOfBirth = employee.DateOfBirth,
                Gender = employee.Gender, Id = employee.Id, Patronymic = employee.Patronymic,
                PhoneNumber = employee.PhoneNumber, Surname = employee.Surname, Status = employee.Status
            });
        }

        [Authorize]
        [Route("profile/me")]
        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployeeDto model)
        {
            if (ModelState.IsValid)
            {
                using HttpClient httpClient = httpClientFactory.CreateClient();
                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await httpClient.PatchAsync($"{url}/api/Employee/UpdateEmployee", jsonContent);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("UpdateEmployee", new { isUpdated = true });
            }

            return View(model);
        }
    }
}
