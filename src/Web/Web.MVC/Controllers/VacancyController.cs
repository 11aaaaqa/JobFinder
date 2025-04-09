using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Web.MVC.Constants.Permissions_constants;
using Web.MVC.DTOs.Vacancy;
using Web.MVC.Filters.Authorization_filters.Company_filters;
using Web.MVC.Models.ApiResponses.Company;
using Web.MVC.Models.ApiResponses.Employer;
using Web.MVC.Models.ApiResponses.Vacancy;

namespace Web.MVC.Controllers
{
    public class VacancyController : Controller
    {
        private readonly string url;
        private readonly IHttpClientFactory httpClientFactory;
        public VacancyController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;   
            url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        }

        [HttpGet]
        [Route("vacancy/all")]
        public async Task<IActionResult> GetAllVacancies(int pageNumber = 1)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{url}/api/Vacancy/GetAllVacancies?pageNumber={pageNumber}");
            response.EnsureSuccessStatusCode();

            var vacancies = await response.Content.ReadFromJsonAsync<List<VacancyResponse>>();

            var doesNextPageExistResponse =
                await httpClient.GetAsync($"{url}/api/Vacancy/DoesNextAllVacanciesPageExist?currentPageNumber={pageNumber}");
            doesNextPageExistResponse.EnsureSuccessStatusCode();

            ViewBag.DoesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            ViewBag.CurrentPageNumber = pageNumber;

            return View(vacancies);
        }

        [Authorize]
        [CompanyPermissionChecker(VacancyPermissionsConstants.AddVacancyPermission)]
        [HttpGet]
        [Route("vacancy/add")]
        public async Task<IActionResult> AddVacancy()
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();

            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var companyResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyId/{employer.CompanyId}");
            companyResponse.EnsureSuccessStatusCode();

            var company = await companyResponse.Content.ReadFromJsonAsync<CompanyResponse>();
            
            return View(new AddVacancyDto
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                CompanyName = company.CompanyName
            });
        }

        [Authorize]
        [CompanyPermissionChecker(VacancyPermissionsConstants.AddVacancyPermission)]
        [HttpPost]
        [Route("vacancy/add")]
        public async Task<IActionResult> AddVacancy(AddVacancyDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.SalaryFrom > model.SalaryTo)
                {
                    ModelState.AddModelError(string.Empty, "Укажите корректную зарплату");
                    return View(model);
                }
                using HttpClient httpClient = httpClientFactory.CreateClient();
                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{url}/api/Vacancy/AddVacancy", jsonContent);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("GetAllVacancies"); //change to redirection to created vacancy
            }
            return View(model);
        }
    }
}
