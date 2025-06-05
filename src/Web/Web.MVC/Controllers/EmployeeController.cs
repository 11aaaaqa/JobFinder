using System.Text;
using System.Text.Json;
using System.Web;
using GeneralLibrary.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.DTOs.Employee;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.Bookmark;
using Web.MVC.Models.ApiResponses.Employee;
using Web.MVC.Models.ApiResponses.Vacancy;

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
        [Route("employee/profile/me")]
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
                PhoneNumber = employee.PhoneNumber, Surname = employee.Surname,
            });
        }

        [Authorize]
        [Route("employee/profile/me")]
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

        [Authorize]
        [Route("employee/update-employee-status")]
        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeStatus(string status, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var getEmployeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            getEmployeeResponse.EnsureSuccessStatusCode();

            var employee = await getEmployeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

            using StringContent jsonContent = new(JsonSerializer.Serialize(new { Status = status, EmployeeId = employee.Id }),
                Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync($"{url}/api/Employee/UpdateEmployeeStatus", jsonContent);
            response.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [Route("employee/favorite/vacancies")]
        [HttpGet]
        public async Task<IActionResult> GetFavoriteVacanciesByEmployeeId(Guid employeeId, string? query, int index = 1)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var encodedQuery = HttpUtility.UrlEncode(query);

            var vacancyResponse = await httpClient.GetAsync(
                $"{url}/api/FavoriteVacancy/GetFavoriteVacancies/{employeeId}?searchingQuery={encodedQuery}&pageNumber={index}");
            vacancyResponse.EnsureSuccessStatusCode();
            var vacancies = await vacancyResponse.Content.ReadFromJsonAsync<List<FavoriteVacancyResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/FavoriteVacancy/DoesNextFavoriteVacanciesPageExist/{employeeId}?searchingQuery={encodedQuery}&currentPageNumber={index}");
            doesNextPageExistResponse.EnsureSuccessStatusCode();
            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.CurrentPageNumber = index;
            ViewBag.DoesNextPageExist = doesNextPageExist;
            ViewBag.SearchingQuery = query;
            ViewBag.EmployeeId = employeeId;

            return View(vacancies);
        }

        [Authorize]
        [Route("employee/responses")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeeVacancyResponses(string? query, DateTimeOrderByType timeSort, int index = 1)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var encodedQuery = HttpUtility.UrlEncode(query);

            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            employeeResponse.EnsureSuccessStatusCode();
            var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

            var vacancyResponsesResponse = await httpClient.GetAsync(
                $"{url}/api/VacancyResponse/GetVacancyResponsesByEmployeeId/{employee.Id}?searchingQuery={encodedQuery}&orderByTimeType={timeSort}&pageNumber={index}");
            vacancyResponsesResponse.EnsureSuccessStatusCode();
            var vacancyResponses = await vacancyResponsesResponse.Content.ReadFromJsonAsync<List<VacancyResponseResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/VacancyResponse/DoesNextVacancyResponsesByEmployeeIdPageExist/{employee.Id}?searchingQuery={encodedQuery}&orderByTimeType={timeSort}&currentPageNumber={index}");
            doesNextPageExistResponse.EnsureSuccessStatusCode();
            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.DoesNextPageExist = doesNextPageExist;
            ViewBag.CurrentPageNumber = index;
            ViewBag.SearchingQuery = query;
            ViewBag.TimeSort = timeSort;

            return View(vacancyResponses);
        }
    }
}
