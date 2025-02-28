using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Web.MVC.DTOs.Company;
using Web.MVC.Models.ApiResponses.Company;
using Web.MVC.Models.ApiResponses.Employer;

namespace Web.MVC.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;
        public CompanyController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        }

        [Authorize]
        [HttpGet]
        [Route("employer/company/my-company")]
        public async Task<IActionResult> GetMyCompany(string? companyQuery)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            if (companyQuery is not null)
            {
                var companyQueryResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyName/{companyQuery}");
                if (companyQueryResponse.IsSuccessStatusCode)
                {
                    var foundCompany = await companyQueryResponse.Content.ReadFromJsonAsync<CompanyResponse>();
                    ViewBag.FoundCompany = foundCompany;
                }
            }

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();

            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            if (employer.CompanyId is null)
                return View();

            var companyResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyId/{employer.CompanyId}");
            companyResponse.EnsureSuccessStatusCode();
            var company = await companyResponse.Content.ReadFromJsonAsync<CompanyResponse>();
            return View(company);
        }

        [Authorize]
        [HttpGet]
        [Route("employer/company/add")]
        public async Task<IActionResult> AddMyCompany()
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            response.EnsureSuccessStatusCode();

            var employer = await response.Content.ReadFromJsonAsync<EmployerResponse>();

            return View(new AddCompanyDto{FounderEmployerId = employer.Id});
        }

        [Authorize]
        [HttpPost]
        [Route("employer/company/add")]
        public async Task<IActionResult> AddMyCompany(AddCompanyDto model)
        {
            if (ModelState.IsValid)
            {
                using HttpClient httpClient = httpClientFactory.CreateClient();
                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{url}/api/Company/AddCompany", jsonContent);
                
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ModelState.AddModelError(string.Empty, "Компания с таким названием уже существует");
                    return View(model);
                }
                response.EnsureSuccessStatusCode();

                return RedirectToAction("GetMyCompany");
            }

            return View(model);
        }
    }
}
