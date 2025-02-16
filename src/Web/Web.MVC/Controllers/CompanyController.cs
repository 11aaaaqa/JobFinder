using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetMyCompany()
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

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
    }
}
