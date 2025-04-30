using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetAllVacancies(string? query, GetFilteredVacancies? filterModel, int index = 1)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            if (filterModel.SalaryFrom is null && filterModel.WorkExperience is null && filterModel.RemoteWork is null &&
                filterModel.EmploymentType is null && filterModel.Position is null && filterModel.VacancyCities is null)
            {
                filterModel = null;
            }
            
            List<VacancyResponse>? vacancies = new();
            if (query is null && filterModel is null)
            {
                var response = await httpClient.GetAsync($"{url}/api/Vacancy/GetAllVacancies?pageNumber={index}");
                response.EnsureSuccessStatusCode();

                vacancies = await response.Content.ReadFromJsonAsync<List<VacancyResponse>>();

                var doesNextPageExistResponse =
                    await httpClient.GetAsync($"{url}/api/Vacancy/DoesNextAllVacanciesPageExist?currentPageNumber={index}");
                doesNextPageExistResponse.EnsureSuccessStatusCode();
                ViewBag.DoesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            }
            else if (query is null && filterModel is not null)
            {
                using StringContent jsonContent = new(JsonSerializer.Serialize(filterModel), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{url}/api/Vacancy/GetFilteredVacancies?pageNumber={index}", jsonContent);
                response.EnsureSuccessStatusCode();

                vacancies = await response.Content.ReadFromJsonAsync<List<VacancyResponse>>();

                var doesNextPageExistResponse = await httpClient.PostAsync(
                    $"{url}/api/Vacancy/DoesNextFilteredVacanciesPageExist?currentPageNumber={index}", jsonContent);
                doesNextPageExistResponse.EnsureSuccessStatusCode();
                ViewBag.DoesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            }
            else if (query is not null && filterModel is not null)
            {
                using StringContent jsonContent = new(JsonSerializer.Serialize(filterModel), Encoding.UTF8, "application/json");
                var encodedQuery = HttpUtility.UrlEncode(query);
                var response = await httpClient.PostAsync($"{url}/api/Vacancy/FindFilteredVacancies?searchingQuery={encodedQuery}&pageNumber={index}",
                    jsonContent);
                response.EnsureSuccessStatusCode();

                vacancies = await response.Content.ReadFromJsonAsync<List<VacancyResponse>>();

                var doesNextPageExistResponse = await httpClient.PostAsync(
                    $"{url}/api/Vacancy/DoesNextSearchFilteredVacanciesPageExist?searchingQuery={encodedQuery}&currentPageNumber={index}", jsonContent);
                doesNextPageExistResponse.EnsureSuccessStatusCode();
                ViewBag.DoesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            }
            else
            {
                var encodedQuery = HttpUtility.UrlEncode(query);
                var findVacanciesResponse = await httpClient.GetAsync(
                    $"{url}/api/Vacancy/FindVacancies?pageNumber={index}&searchingQuery={encodedQuery}");
                findVacanciesResponse.EnsureSuccessStatusCode();

                vacancies = await findVacanciesResponse.Content.ReadFromJsonAsync<List<VacancyResponse>>();

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"{url}/api/Vacancy/DoesNextSearchVacanciesPageExist?searchingQuery={encodedQuery}&currentPageNumber={index}");
                doesNextPageExistResponse.EnsureSuccessStatusCode();
                ViewBag.DoesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            }

            ViewBag.CurrentPageNumber = index;
            ViewBag.SearchingQuery = query;
            ViewBag.FilterModel = filterModel;

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

                return RedirectToAction("GetVacancy", new { vacancyId = model.Id});
            }
            return View(model);
        }

        [HttpGet]
        [Route("vacancy/{vacancyId}")]
        public async Task<IActionResult> GetVacancy(Guid vacancyId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"{url}/api/Vacancy/GetVacancyById/{vacancyId}");
            response.EnsureSuccessStatusCode();

            var vacancy = await response.Content.ReadFromJsonAsync<VacancyResponse>();

            if (User.Identity.IsAuthenticated)
            {
                var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
                if (!employerResponse.IsSuccessStatusCode)
                    return View(vacancy);

                var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();
                if(vacancy.CompanyId != employer.CompanyId)
                    return View(vacancy);

                var companyResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyId/{employer.CompanyId}");
                companyResponse.EnsureSuccessStatusCode();

                var company = await companyResponse.Content.ReadFromJsonAsync<CompanyResponse>();

                if (company.FounderEmployerId == employer.Id)
                {
                    ViewBag.DoesEmployerHavePermissionToDeleteVacancies = true;
                    ViewBag.DoesEmployerHavePermissionToArchiveAndUnarchiveVacancies = true;
                    ViewBag.DoesEmployerHavePermissionToEditVacancies = true;
                }
                else
                {
                    var employerPermissionsResponse = await httpClient.GetAsync($"{url}/api/EmployerPermissions/GetEmployerPermissions/{employer.Id}");
                    employerPermissionsResponse.EnsureSuccessStatusCode();

                    var employerPermissions = await employerPermissionsResponse.Content.ReadFromJsonAsync<List<string>>();
                    ViewBag.DoesEmployerHavePermissionToDeleteVacancies = employerPermissions.Contains(VacancyPermissionsConstants.DeleteVacancyPermission);
                    ViewBag.DoesEmployerHavePermissionToArchiveAndUnarchiveVacancies = employerPermissions.Contains(VacancyPermissionsConstants.ArchiveUnarchiveVacancyPermission);
                    ViewBag.DoesEmployerHavePermissionToEditVacancies = employerPermissions.Contains(VacancyPermissionsConstants.EditVacancyPermission);
                }
            }
            
            return View(vacancy);
        }

        [HttpGet]
        [Route("vacancy/search/advanced")]
        public IActionResult SetVacancyAdvancedFilter(string? position, int? salaryFrom, string? workExperience, string? employmentType, bool officeWorkType,
            bool remoteWorkType, List<string>? vacancyCities)
        {
            return View(new SetVacancyAdvancedFilterDto
            {
                Position = position, EmploymentType = employmentType, OfficeWorkType = officeWorkType,
                RemoteWorkType = remoteWorkType, SalaryFrom = salaryFrom, WorkExperience = workExperience, VacancyCities = vacancyCities
            });
        }

        [HttpPost]
        [Route("vacancy/search/advanced")]
        public IActionResult SetVacancyAdvancedFilter(SetVacancyAdvancedFilterDto model)
        {
            if (ModelState.IsValid)
            {
                if (!model.OfficeWorkType && model.RemoteWorkType)
                {
                    return RedirectToAction("GetAllVacancies", new
                    {
                        model.SalaryFrom,
                        model.WorkExperience,
                        model.EmploymentType,
                        model.Position,
                        model.VacancyCities,
                        RemoteWork = true
                    });
                }
                return RedirectToAction("GetAllVacancies", new
                {
                    model.SalaryFrom,
                    model.WorkExperience,
                    model.EmploymentType,
                    model.Position,
                    model.VacancyCities
                });
            }

            return View(model);
        }
    }
}
