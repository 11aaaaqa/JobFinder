using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
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

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();

            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            if (companyQuery is not null)
            {
                var companyQueryResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyName/{companyQuery}");
                if (companyQueryResponse.IsSuccessStatusCode)
                {
                    var foundCompany = await companyQueryResponse.Content.ReadFromJsonAsync<CompanyResponse>();
                    var didAlreadyRequestJoiningResponse = await httpClient.GetAsync(
                        $"{url}/api/CompanyEmployer/DidEmployerRequestJoining?employerId={employer.Id}&companyId={foundCompany.Id}");
                    didAlreadyRequestJoiningResponse.EnsureSuccessStatusCode();
                    bool employerAlreadyRequestedJoining =
                        await didAlreadyRequestJoiningResponse.Content.ReadFromJsonAsync<bool>();
                    ViewBag.EmployerAlreadyRequestedJoining = employerAlreadyRequestedJoining;
                    ViewBag.FoundCompany = foundCompany;
                }
            }

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

        [Authorize]
        [HttpPost]
        [Route("employer/company/request-joining")]
        public async Task<IActionResult> RequestJoiningCompany(Guid companyId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            response.EnsureSuccessStatusCode();

            var employer = await response.Content.ReadFromJsonAsync<EmployerResponse>();

            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                CompanyId = companyId, EmployerId = employer.Id, EmployerName = employer.Name, EmployerSurname = employer.Surname
            }), Encoding.UTF8, "application/json");
            var requestJoiningResponse = await httpClient.PostAsync($"{url}/api/CompanyEmployer/RequestJoiningCompany", jsonContent);
            if (!requestJoiningResponse.IsSuccessStatusCode)
                return View("JoiningIsAlreadyRequested");

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [HttpGet]
        [Route("employer/company/my-company/update")]
        public async Task<IActionResult> UpdateMyCompany(bool? isUpdated)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var companyResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyId/{employer.CompanyId}");
            companyResponse.EnsureSuccessStatusCode();
            var company = await companyResponse.Content.ReadFromJsonAsync<CompanyResponse>();

            ViewBag.IsUpdated = isUpdated;

            return View(new UpdateCompanyDto
            {
                CompanyColleaguesCount = company.CompanyColleaguesCount, CompanyDescription = company.CompanyDescription,
                CompanyName = company.CompanyName, Id = company.Id
            });
        }

        [Authorize]
        [HttpPost]
        [Route("employer/company/my-company/update")]
        public async Task<IActionResult> UpdateMyCompany(UpdateCompanyDto model)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync($"{url}/api/Company/UpdateCompany", jsonContent);
            response.EnsureSuccessStatusCode();
            return RedirectToAction("UpdateMyCompany", new { isUpdated = true });
        }

        [Authorize]
        [HttpGet]
        [Route("employer/company/my-company/employers-requested-joining")] 
        public async Task<IActionResult> GetEmployersRequestedJoiningMyCompany(int index = 1)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();

            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var companyId = employer.CompanyId;
            var nextPage = index + 1;

            var employersRequestedJoiningResponse = await httpClient.GetAsync(
                $"{url}/api/CompanyEmployer/GetListOfEmployersRequestedJoining/{companyId}?pageNumber={index}");
            employersRequestedJoiningResponse.EnsureSuccessStatusCode();

            var employers = await employersRequestedJoiningResponse.Content.ReadFromJsonAsync<List<JoiningRequestedEmployerResponse>>();

            var nextPageExistsResponse = await httpClient.GetAsync(
                $"{url}/api/CompanyEmployer/DoesEmployersRequestedJoiningPageExist/{companyId}?pageNumber={nextPage}");
            nextPageExistsResponse.EnsureSuccessStatusCode();

            var doesNextPageExist = await nextPageExistsResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.CurrentPageNumber = index;
            ViewBag.DoesNextPageExist = doesNextPageExist;

            return View(employers);
        }

        [Authorize]
        [HttpPost]
        [Route("employer/company/my-company/accept-employer-requested-joining")]
        public async Task<IActionResult> AcceptEmployerToJoinCompany(Guid requestId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"{url}/api/CompanyEmployer/AcceptEmployerJoiningRequest/{requestId}");
            response.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [HttpPost]
        [Route("employer/company/my-company/reject-employer-requested-joining")]
        public async Task<IActionResult> RejectEmployerToJoinCompany(Guid requestId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"{url}/api/CompanyEmployer/RejectEmployerJoiningRequest/{requestId}");
            response.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [HttpGet]
        [Route("employer/company/my-company/colleagues")]
        public async Task<IActionResult> GetMyCompanyColleagues(int index = 1)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();

            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var colleaguesResponse = await httpClient.GetAsync(
                $"{url}/api/Employer/GetEmployersByCompanyId/{employer.CompanyId}?pageNumber={index}");
            colleaguesResponse.EnsureSuccessStatusCode();

            var colleagues = await colleaguesResponse.Content.ReadFromJsonAsync<List<EmployerResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/Employer/DoesNextEmployersByCompanyIdPageExist/{employer.CompanyId}?currentPageNumber={index}");
            doesNextPageExistResponse.EnsureSuccessStatusCode();

            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.DoesNextPageExist = doesNextPageExist;
            ViewBag.CurrentPageNumber = index;

            return View(colleagues);
        }
    }
}
