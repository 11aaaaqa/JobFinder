using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Constants.Permissions_constants;
using Web.MVC.DTOs.Vacancy;
using Web.MVC.Filters.Authorization_filters.Company_filters;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.Company;
using Web.MVC.Models.ApiResponses.Employer;
using Web.MVC.Models.ApiResponses.Resume;
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
            if (response.StatusCode == HttpStatusCode.NotFound)
                return StatusCode((int)HttpStatusCode.NotFound);
            response.EnsureSuccessStatusCode();

            var vacancy = await response.Content.ReadFromJsonAsync<VacancyResponse>();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            
            if (User.Identity.IsAuthenticated && employerResponse.IsSuccessStatusCode)
            {
                var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();
                if(vacancy.CompanyId != employer.CompanyId)
                    return View(vacancy);

                ViewBag.IsEmployerVacancyOwner = true;

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

            if (User.Identity.IsAuthenticated && employeeResponse.IsSuccessStatusCode)
            {
                var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployerResponse>();

                var favoriteVacancyResponse = await httpClient.GetAsync(
                    $"{url}/api/FavoriteVacancy/IsVacancyInFavorites?employeeId={employee.Id}&vacancyId={vacancyId}");
                favoriteVacancyResponse.EnsureSuccessStatusCode();

                var employeeRespondedToVacancyResponse = await httpClient.GetAsync(
                    $"{url}/api/VacancyResponse/HasEmployeeRespondedToVacancy?employeeId={employee.Id}&vacancyId={vacancyId}");
                employeeRespondedToVacancyResponse.EnsureSuccessStatusCode();
                bool hasEmployeeRespondedToVacancy = await employeeRespondedToVacancyResponse.Content.ReadFromJsonAsync<bool>();

                var interviewInvitation = await httpClient.GetAsync(
                    $"{url}/api/InterviewInvitation/GetInterviewInvitation?employeeId={employee.Id}&companyId={vacancy.CompanyId}");

                ViewBag.EmployeeGotInvitedToThisVacancy = interviewInvitation.IsSuccessStatusCode;
                ViewBag.HasEmployeeRespondedToVacancy = hasEmployeeRespondedToVacancy;

                bool isVacancyInFavorites = await favoriteVacancyResponse.Content.ReadFromJsonAsync<bool>();
                ViewBag.IsVacancyInFavorites = isVacancyInFavorites;
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

        [Authorize]
        [HttpPost]
        [Route("vacancy/favorite/add")]
        public async Task<IActionResult> AddVacancyToFavorites(Guid vacancyId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employeeResponse = await httpClient.GetAsync(
                $"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            employeeResponse.EnsureSuccessStatusCode();
            var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

            var vacancyResponse = await httpClient.GetAsync($"{url}/api/Vacancy/GetVacancyById/{vacancyId}");
            vacancyResponse.EnsureSuccessStatusCode();
            var vacancy = await vacancyResponse.Content.ReadFromJsonAsync<VacancyResponse>();

            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                EmployeeId = employee.Id, VacancyId = vacancy.Id,
                vacancy.Position, vacancy.SalaryFrom, vacancy.SalaryTo,
                vacancy.WorkExperience, vacancy.CompanyName, vacancy.VacancyCity
            }), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{url}/api/FavoriteVacancy/AddToFavorites", jsonContent);
            response.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [HttpPost]
        [Route("vacancy/favorite/remove")]
        public async Task<IActionResult> RemoveVacancyFromFavorites(Guid vacancyId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            employeeResponse.EnsureSuccessStatusCode();
            var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var response = await httpClient.DeleteAsync(
                $"{url}/api/FavoriteVacancy/DeleteFromFavorites?employeeId={employee.Id}&vacancyId={vacancyId}");
            response.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [HttpGet]
        [Route("vacancy/{vacancyId}/respond")]
        public async Task<IActionResult> RespondToVacancy(Guid vacancyId, Guid resumeId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            employeeResponse.EnsureSuccessStatusCode();
            var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var vacancyResponse = await httpClient.GetAsync($"{url}/api/Vacancy/GetVacancyById/{vacancyId}");
            vacancyResponse.EnsureSuccessStatusCode();
            var vacancy = await vacancyResponse.Content.ReadFromJsonAsync<VacancyResponse>();

            var resumeResponse = await httpClient.GetAsync($"{url}/api/Resume/GetResumeById/{resumeId}");
            resumeResponse.EnsureSuccessStatusCode();
            var resume = await resumeResponse.Content.ReadFromJsonAsync<ResumeResponse>();

            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                VacancyCompanyId = vacancy.CompanyId,
                EmployeeId = employee.Id,
                RespondedEmployeeResumeId = resumeId,
                EmployeeName = employee.Name,
                EmployeeSurname = employee.Surname,
                EmployeeDesiredSalary = resume.DesiredSalary,
                EmployeeDateOfBirth = resume.DateOfBirth,
                EmployeeWorkingExperience = resume.WorkingExperience,
                EmployeeCity = resume.City,
                VacancyId = vacancyId,
                VacancyPosition = vacancy.Position,
                VacancySalaryFrom = vacancy.SalaryFrom,
                VacancySalaryTo = vacancy.SalaryTo,
                VacancyWorkExperience = vacancy.WorkExperience,
                VacancyCity = vacancy.VacancyCity,
                VacancyCompanyName = vacancy.CompanyName
            }), Encoding.UTF8, "application/json");
            var respondToVacancyResponse = await httpClient.PostAsync($"{url}/api/VacancyResponse/AddVacancyResponse", jsonContent);
            respondToVacancyResponse.EnsureSuccessStatusCode();

            return RedirectToAction("GetVacancy", new { vacancyId });
        }

        [Authorize]
        [HttpGet]
        [Route("vacancy/{vacancyId}/respond/choose-resume")]
        public async Task<IActionResult> ChooseResumeToRespondToVacancy(Guid vacancyId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            employeeResponse.EnsureSuccessStatusCode();
            var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var resumeResponse = await httpClient.GetAsync($"{url}/api/Resume/GetResumesByEmployeeId/{employee.Id}");
            resumeResponse.EnsureSuccessStatusCode();
            var resumes = await resumeResponse.Content.ReadFromJsonAsync<List<ResumeResponse>>();

            if (resumes.Count == 0)
            {
                string returnUrl = Request.Path;
                if (Request.QueryString.HasValue)
                    returnUrl += Request.QueryString.Value;
                return RedirectToAction("AddResume", "Resume", new { returnUrl });
            }
                

            if (resumes.Count == 1)
                return RedirectToAction("RespondToVacancy", new { vacancyId, resumeId = resumes[0].Id });

            ViewBag.VacancyId = vacancyId;

            return View(resumes);
        }

        [Authorize]
        [HttpPost]
        [Route("vacancy/{vacancyId}/respond/choose-resume")]
        public IActionResult ChooseResumeToRespondToVacancy(Guid vacancyId, Guid resumeId)
        {
            return RedirectToAction("RespondToVacancy", new { vacancyId, resumeId});
        }
    }
}
