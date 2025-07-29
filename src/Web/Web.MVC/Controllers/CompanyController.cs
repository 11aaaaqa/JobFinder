using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Web;
using GeneralLibrary.Constants;
using GeneralLibrary.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web.MVC.Chat_services;
using Web.MVC.Constants.Permissions_constants;
using Web.MVC.DTOs.Company;
using Web.MVC.DTOs.Vacancy;
using Web.MVC.Filters.Authorization_filters.Company_filters;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.Chat;
using Web.MVC.Models.ApiResponses.Company;
using Web.MVC.Models.ApiResponses.Employer;
using Web.MVC.Models.ApiResponses.Response;
using Web.MVC.Models.ApiResponses.Resume;
using Web.MVC.Models.ApiResponses.Vacancy;
using Web.MVC.Services.Notification_services;

namespace Web.MVC.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;
        private readonly IHubContext<ChatHub> chatHub;
        private readonly INotificationService notificationService;
        public CompanyController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHubContext<ChatHub> chatHub,
            INotificationService notificationService)
        {
            this.httpClientFactory = httpClientFactory;
            this.chatHub = chatHub;
            this.notificationService = notificationService;
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

            if (companyQuery is not null && employer.CompanyId is null)
            {
                var encodedQuery = HttpUtility.UrlEncode(companyQuery);
                var companyQueryResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyName?companyName={encodedQuery}");
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

            if (employer.CompanyId is not null)
                return View("EmployerIsAlreadyInCompanyError");

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

                var removeEmployerJoiningRequestsResponse = await httpClient.GetAsync(
                    $"{url}/api/CompanyEmployer/RemoveAllEmployerRequestsByEmployerId/{model.FounderEmployerId}");
                removeEmployerJoiningRequestsResponse.EnsureSuccessStatusCode();

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

            if (employer.CompanyId is not null)
                return View("EmployerIsAlreadyInCompanyError");

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
        [CompanyPermissionChecker(CompanyPermissionsConstants.UpdateCompanyInformationDataPermission)]
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
        [CompanyPermissionChecker(CompanyPermissionsConstants.UpdateCompanyInformationDataPermission)]
        [HttpPost]
        [Route("employer/company/my-company/update")]
        public async Task<IActionResult> UpdateMyCompany(UpdateCompanyDto model)
        {
            if (ModelState.IsValid)
            {
                using HttpClient httpClient = httpClientFactory.CreateClient();
                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                var response = await httpClient.PatchAsync($"{url}/api/Company/UpdateCompany", jsonContent);
                response.EnsureSuccessStatusCode();
                return RedirectToAction("UpdateMyCompany", new { isUpdated = true });
            }

            return View(model);
        }

        [Authorize]
        [CompanyPermissionChecker(CompanyPermissionsConstants.ViewListOfEmployersRequestedJoiningPermission)]
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
        [CompanyPermissionChecker(CompanyPermissionsConstants.AcceptRejectEmployersRequestedJoiningPermission)]
        [HttpPost]
        [Route("employer/company/my-company/accept-employer-requested-joining")]
        public async Task<IActionResult> AcceptEmployerToJoinCompany(Guid requestId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var requestResponse = await httpClient.GetAsync($"{url}/api/CompanyEmployer/GetJoiningRequestByRequestId/{requestId}");
            requestResponse.EnsureSuccessStatusCode();

            var request = await requestResponse.Content.ReadFromJsonAsync<JoiningRequestedEmployerResponse>();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            if (employer.CompanyId != request.CompanyId)
                return RedirectToAction("AccessForbidden", "Information");

            using StringContent jsonContent = new(JsonSerializer.Serialize(new { request.EmployerId , request.CompanyId}),
                Encoding.UTF8, "application/json");
            var assignCompanyResponse = await httpClient.PatchAsync($"{url}/api/Employer/AssignCompany", jsonContent);
            assignCompanyResponse.EnsureSuccessStatusCode();

            var removeAllEmployerRequestsResponse = await httpClient.GetAsync(
                $"{url}/api/CompanyEmployer/RemoveAllEmployerRequestsByEmployerId/{request.EmployerId}");
            removeAllEmployerRequestsResponse.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [CompanyPermissionChecker(CompanyPermissionsConstants.AcceptRejectEmployersRequestedJoiningPermission)]
        [HttpPost]
        [Route("employer/company/my-company/reject-employer-requested-joining")]
        public async Task<IActionResult> RejectEmployerToJoinCompany(Guid requestId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var requestResponse = await httpClient.GetAsync($"{url}/api/CompanyEmployer/GetJoiningRequestByRequestId/{requestId}");
            requestResponse.EnsureSuccessStatusCode();
            var request = await requestResponse.Content.ReadFromJsonAsync<JoiningRequestedEmployerResponse>();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            if (employer.CompanyId != request.CompanyId)
                return RedirectToAction("AccessForbidden", "Information");

            var response = await httpClient.GetAsync($"{url}/api/CompanyEmployer/RejectEmployerJoiningRequest/{requestId}");
            response.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [CompanyPermissionChecker(ColleaguesPermissionsConstants.ViewTheListOfColleaguesPermission)]
        [HttpGet]
        [Route("employer/company/my-company/colleagues")]
        public async Task<IActionResult> GetMyCompanyColleagues(string? query, int index = 1)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();

            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var companyResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyId/{employer.CompanyId}");
            companyResponse.EnsureSuccessStatusCode();
            var company = await companyResponse.Content.ReadFromJsonAsync<CompanyResponse>();

            if (company.FounderEmployerId == employer.Id)
            {
                ViewBag.DoesEmployerHasPermissionToViewColleaguesPermissions = true;
            }
            else
            {
                var employerPermissionResponse = await httpClient.GetAsync(
                    $"{url}/api/EmployerPermissions/CheckForPermission/{employer.Id}?permissionName={ColleaguesPermissionsConstants.ViewColleaguesPermissionsPermission}");
                employerPermissionResponse.EnsureSuccessStatusCode();
                bool doesEmployerHasPermissionToViewColleaguesPermissions = await employerPermissionResponse.Content.ReadFromJsonAsync<bool>();
                ViewBag.DoesEmployerHasPermissionToViewColleaguesPermissions = doesEmployerHasPermissionToViewColleaguesPermissions;
            }

            bool doesNextPageExist;
            List<EmployerResponse>? colleagues = new List<EmployerResponse>();
            if (query is null)
            {
                var colleaguesResponse = await httpClient.GetAsync(
                    $"{url}/api/Employer/GetEmployersByCompanyId/{employer.CompanyId}?pageNumber={index}");
                colleaguesResponse.EnsureSuccessStatusCode();

                colleagues = await colleaguesResponse.Content.ReadFromJsonAsync<List<EmployerResponse>>();

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"{url}/api/Employer/DoesNextEmployersByCompanyIdPageExist/{employer.CompanyId}?currentPageNumber={index}");
                doesNextPageExistResponse.EnsureSuccessStatusCode();

                doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            }
            else
            {
                var colleaguesResponse = await httpClient.GetAsync(
                    $"{url}/api/Employer/FindEmployers/{employer.CompanyId}?pageNumber={index}&searchingQuery={query}");
                colleaguesResponse.EnsureSuccessStatusCode();

                colleagues = await colleaguesResponse.Content.ReadFromJsonAsync<List<EmployerResponse>>();

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"{url}/api/Employer/DoesNextFindEmployersPageExist/{employer.CompanyId}?currentPageNumber={index}&searchingQuery={query}");
                doesNextPageExistResponse.EnsureSuccessStatusCode();

                doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            }

            ViewBag.SearchingQuery = query;
            ViewBag.DoesNextPageExist = doesNextPageExist;
            ViewBag.CurrentPageNumber = index;
            ViewBag.FounderEmployerId = company.FounderEmployerId;

            return View(colleagues);
        }

        [Authorize]
        [CompanyPermissionChecker(ColleaguesPermissionsConstants.RemoveColleaguesPermission)]
        [HttpPost]
        [Route("employer/company/my-company/colleagues/remove-from-company")]
        public async Task<IActionResult> RemoveEmployerFromCompany(Guid employerId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var removingEmployerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            removingEmployerResponse.EnsureSuccessStatusCode();
            var removingEmployer = await removingEmployerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var beingRemovedEmployerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerById/{employerId}");
            beingRemovedEmployerResponse.EnsureSuccessStatusCode();
            var beingRemovedEmployer = await beingRemovedEmployerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            if (removingEmployer.CompanyId != beingRemovedEmployer.CompanyId)
                return RedirectToAction("AccessForbidden","Information");

            var response = await httpClient.GetAsync($"{url}/api/Employer/RemoveEmployerFromCompany/{employerId}");
            response.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [HttpPost]
        [Route("employer/company/my-company/leave")]
        public async Task<IActionResult> LeaveFromCompany(string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();

            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var companyResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyId/{employer.CompanyId}");
            companyResponse.EnsureSuccessStatusCode();
            var company = await companyResponse.Content.ReadFromJsonAsync<CompanyResponse>();

            if (company.FounderEmployerId == employer.Id)
                return View("ActionError");

            var removeEmployerFromCompanyResponse = await httpClient.GetAsync($"{url}/api/Employer/RemoveEmployerFromCompany/{employer.Id}");
            removeEmployerFromCompanyResponse.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [CompanyPermissionChecker(ColleaguesPermissionsConstants.ViewColleaguesPermissionsPermission)]
        [HttpGet]
        [Route("employer/company/my-company/colleagues/update-permissions")]
        public async Task<IActionResult> UpdateEmployerCompanyPermissions(Guid employerId, bool? isUpdated)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerById/{employerId}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();
            
            if (employer.Email == User.Identity.Name)
                return View("ActionError");

            ViewBag.EmployerName = employer.Name; ViewBag.EmployerSurname = employer.Surname; 
            ViewBag.EmployerEmail = employer.Email; ViewBag.EmployerCompanyPost = employer.CompanyPost;
            ViewBag.IsUpdated = isUpdated;

            var employerPermissionsResponse = await httpClient.GetAsync($"{url}/api/EmployerPermissions/GetEmployerPermissions/{employerId}");
            if (employerPermissionsResponse.StatusCode == HttpStatusCode.NotFound)
                return View(new UpdateEmployerCompanyPermissionsDto{EmployerId = employerId, Permissions = new List<string>()});
            employerPermissionsResponse.EnsureSuccessStatusCode();
            var employerPermissions = await employerPermissionsResponse.Content.ReadFromJsonAsync<List<string>>();

            return View(new UpdateEmployerCompanyPermissionsDto{EmployerId = employerId, Permissions = employerPermissions});
        }

        [Authorize]
        [CompanyPermissionChecker(ColleaguesPermissionsConstants.GivePermissionsToColleaguesPermission)]
        [HttpPost]
        [Route("employer/company/my-company/colleagues/update-permissions")]
        public async Task<IActionResult> UpdateEmployerCompanyPermissions(UpdateEmployerCompanyPermissionsDto model)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var updatingEmployerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            updatingEmployerResponse.EnsureSuccessStatusCode();
            var updatingEmployer = await updatingEmployerResponse.Content.ReadFromJsonAsync<EmployerResponse>();
            
            var beingUpdatedEmployerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerById/{model.EmployerId}");
            beingUpdatedEmployerResponse.EnsureSuccessStatusCode();
            var beingUpdatedEmployer = await beingUpdatedEmployerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            if (updatingEmployer.CompanyId != beingUpdatedEmployer.CompanyId)
                return RedirectToAction("AccessForbidden","Information");

            using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{url}/api/EmployerPermissions/AddPermissions", jsonContent);
            response.EnsureSuccessStatusCode();

            return RedirectToAction("UpdateEmployerCompanyPermissions", new { isUpdated = true, employerId = model.EmployerId });
        }

        [Authorize]
        [CompanyPermissionChecker(CompanyPermissionsConstants.DeleteCompanyPermission)]
        [HttpPost]
        [Route("employer/company/my-company/delete")]
        public async Task<IActionResult> DeleteMyCompany()
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();

            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();
            
            var deleteCompanyResponse = await httpClient.DeleteAsync($"{url}/api/Company/DeleteCompany/{employer.CompanyId}");
            deleteCompanyResponse.EnsureSuccessStatusCode();
            return RedirectToAction("GetMyCompany", "Company");
        }

        [Authorize]
        [HttpGet]
        [Route("employer/company/my-company/vacancies")]
        public async Task<IActionResult> GetMyCompanyVacancies(string? query, int? index = 1)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var encodedQuery = HttpUtility.UrlEncode(query);

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();

            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var vacanciesResponse = await httpClient.GetAsync(
                $"{url}/api/Vacancy/GetVacanciesByCompanyId/{employer.CompanyId}?pageNumber={index}&searchingQuery={encodedQuery}");
            vacanciesResponse.EnsureSuccessStatusCode();

            var vacancies = await vacanciesResponse.Content.ReadFromJsonAsync<List<VacancyResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/Vacancy/DoesNextVacanciesByCompanyIdPageExist/{employer.CompanyId}?currentPageNumber={index}&searchingQuery={encodedQuery}");
            doesNextPageExistResponse.EnsureSuccessStatusCode();

            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.SearchingQuery = query;
            ViewBag.CurrentPageNumber = index;
            ViewBag.DoesNextPageExist = doesNextPageExist;

            return View(vacancies);
        }

        [Authorize]
        [CompanyPermissionChecker(VacancyPermissionsConstants.DeleteVacancyPermission)]
        [HttpPost]
        [Route("employer/company/my-company/vacancies/{vacancyId}/remove")]
        public async Task<IActionResult> DeleteVacancy(Guid vacancyId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var vacancyResponse = await httpClient.GetAsync($"{url}/api/Vacancy/GetVacancyById/{vacancyId}");
            vacancyResponse.EnsureSuccessStatusCode();
            var vacancy = await vacancyResponse.Content.ReadFromJsonAsync<VacancyResponse>();

            if (employer.CompanyId != vacancy.CompanyId)
                return RedirectToAction("AccessForbidden", "Information");

            var response = await httpClient.DeleteAsync($"{url}/api/Vacancy/DeleteVacancy/{vacancyId}");
            response.EnsureSuccessStatusCode();

            return RedirectToAction("GetMyCompanyVacancies");
        }

        [Authorize]
        [CompanyPermissionChecker(VacancyPermissionsConstants.ArchiveUnarchiveVacancyPermission)]
        [HttpPost]
        [Route("employer/company/my-company/vacancies/{vacancyId}/archive")]
        public async Task<IActionResult> ArchiveVacancy(Guid vacancyId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var vacancyResponse = await httpClient.GetAsync($"{url}/api/Vacancy/GetVacancyById/{vacancyId}");
            vacancyResponse.EnsureSuccessStatusCode();
            var vacancy = await vacancyResponse.Content.ReadFromJsonAsync<VacancyResponse>();

            if (employer.CompanyId != vacancy.CompanyId)
                return RedirectToAction("AccessForbidden", "Information");

            var response = await httpClient.GetAsync($"{url}/api/Vacancy/ArchiveVacancy/{vacancyId}");
            response.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [CompanyPermissionChecker(VacancyPermissionsConstants.ArchiveUnarchiveVacancyPermission)]
        [HttpPost]
        [Route("employer/company/my-company/archived-vacancies/{vacancyId}/unarchive")]
        public async Task<IActionResult> UnarchiveVacancy(Guid vacancyId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var vacancyResponse = await httpClient.GetAsync($"{url}/api/Vacancy/GetVacancyById/{vacancyId}");
            vacancyResponse.EnsureSuccessStatusCode();
            var vacancy = await vacancyResponse.Content.ReadFromJsonAsync<VacancyResponse>();

            if (employer.CompanyId != vacancy.CompanyId)
                return RedirectToAction("AccessForbidden", "Information");

            var response = await httpClient.GetAsync($"{url}/api/Vacancy/UnarchiveVacancy/{vacancyId}");
            response.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [CompanyPermissionChecker(VacancyPermissionsConstants.EditVacancyPermission)]
        [HttpGet]
        [Route("employer/company/my-company/vacancies/edit/{vacancyId}")]
        public async Task<IActionResult> EditVacancy(Guid vacancyId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"{url}/api/Vacancy/GetVacancyById/{vacancyId}");
            response.EnsureSuccessStatusCode();

            var vacancy = await response.Content.ReadFromJsonAsync<VacancyResponse>();

            return View(new EditVacancyDto
            {
                Id = vacancy.Id, SalaryTo = vacancy.SalaryTo, Address = vacancy.Address, Description = vacancy.Description,
                EmployerContactEmail = vacancy.EmployerContactEmail, EmployerContactPhoneNumber = vacancy.EmployerContactPhoneNumber,
                EmploymentType = vacancy.EmploymentType, Position = vacancy.Position, RemoteWork = vacancy.RemoteWork,
                SalaryFrom = vacancy.SalaryFrom, VacancyCity = vacancy.VacancyCity, WorkExperience = vacancy.WorkExperience
            });
        }

        [Authorize]
        [CompanyPermissionChecker(VacancyPermissionsConstants.EditVacancyPermission)]
        [HttpPost]
        [Route("employer/company/my-company/vacancies/edit/{vacancyId}")]
        public async Task<IActionResult> EditVacancy(EditVacancyDto model)
        {
            if (ModelState.IsValid)
            {
                using HttpClient httpClient = httpClientFactory.CreateClient();

                var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
                employerResponse.EnsureSuccessStatusCode();
                var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

                var vacancyResponse = await httpClient.GetAsync($"{url}/api/Vacancy/GetVacancyById/{model.Id}");
                vacancyResponse.EnsureSuccessStatusCode();
                var vacancy = await vacancyResponse.Content.ReadFromJsonAsync<VacancyResponse>();

                if (employer.CompanyId != vacancy.CompanyId)
                    return RedirectToAction("AccessForbidden","Information");

                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"{url}/api/Vacancy/UpdateVacancy", jsonContent);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("GetVacancy", "Vacancy", new { vacancyId = model.Id});
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        [Route("employer/company/my-company/vacancies/archived")]
        public async Task<IActionResult> GetMyCompanyArchivedVacancies(string? query, int index = 1)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var encodedQuery = HttpUtility.UrlEncode(query);

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();

            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var vacanciesResponse = await httpClient.GetAsync(
                $"{url}/api/Vacancy/GetArchivedVacanciesByCompanyId/{employer.CompanyId}?pageNumber={index}&searchingQuery={encodedQuery}");
            vacanciesResponse.EnsureSuccessStatusCode();

            var vacancies = await vacanciesResponse.Content.ReadFromJsonAsync<List<VacancyResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/Vacancy/DoesNextArchivedVacanciesByCompanyIdPageExist/{employer.CompanyId}?searchingQuery={encodedQuery}&currentPageNumber={index}");
            doesNextPageExistResponse.EnsureSuccessStatusCode();

            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.SearchingQuery = query;
            ViewBag.CurrentPageNumber = index;
            ViewBag.DoesNextPageExist = doesNextPageExist;

            return View(vacancies);
        }

        [Authorize]
        [HttpGet]
        [Route("employer/company/my-company/vacancy-responses")]
        public async Task<IActionResult> GetCompanyVacancyResponses(DateTimeOrderByType timeSort, int index = 1)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var vacancyResponsesResponse = await httpClient.GetAsync(
                $"{url}/api/VacancyResponse/GetVacancyResponsesByCompanyId/{employer.CompanyId}?orderByTimeType={timeSort}&pageNumber={index}");
            vacancyResponsesResponse.EnsureSuccessStatusCode();
            var vacancyResponses = await vacancyResponsesResponse.Content.ReadFromJsonAsync<List<VacancyResponseResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/VacancyResponse/DoesNextVacancyResponsesByCompanyIdPageExist/{employer.CompanyId}?orderByTimeType={timeSort}&currentPageNumber={index}");
            doesNextPageExistResponse.EnsureSuccessStatusCode();
            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.DoesNextPageExist = doesNextPageExist;
            ViewBag.CurrentPageNumber = index;
            ViewBag.TimeSort = timeSort;

            return View(vacancyResponses);
        }

        [Authorize]
        [HttpGet]
        [Route("vacancy/{vacancyId}/responses")]
        public async Task<IActionResult> GetVacancyResponsesByVacancyId(Guid vacancyId, DateTimeOrderByType timeSort, int index = 1)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var vacancyResponse = await httpClient.GetAsync($"{url}/api/Vacancy/GetVacancyById/{vacancyId}");
            vacancyResponse.EnsureSuccessStatusCode();
            var vacancy = await vacancyResponse.Content.ReadFromJsonAsync<VacancyResponse>();

            if (vacancy.CompanyId != employer.CompanyId)
                return RedirectToAction("AccessForbidden", "Information");

            var vacancyResponsesResponse = await httpClient.GetAsync(
                $"{url}/api/VacancyResponse/GetCompanyVacancyResponsesByVacancyId/{vacancyId}?orderByTimeType={timeSort}&pageNumber={index}");
            vacancyResponsesResponse.EnsureSuccessStatusCode();
            var vacancyResponses = await vacancyResponsesResponse.Content.ReadFromJsonAsync<List<VacancyResponseResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/VacancyResponse/DoesNextCompanyVacancyResponsesByVacancyIdPageExist/{vacancyId}?orderByTimeType={timeSort}&currentPageNumber={index}");
            doesNextPageExistResponse.EnsureSuccessStatusCode();
            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.DoesNextPageExist = doesNextPageExist;
            ViewBag.CurrentPageNumber = index;
            ViewBag.TimeSort = timeSort;
            ViewBag.VacancyId = vacancyId;

            return View(vacancyResponses);
        }

        [Authorize]
        [HttpPost]
        [Route("vacancy-responses/{vacancyResponseId}/accept")]
        public async Task<IActionResult> AcceptVacancyResponse(Guid vacancyResponseId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var vacancyResponseResponse = await httpClient.GetAsync($"{url}/api/VacancyResponse/GetVacancyResponseById/{vacancyResponseId}");
            vacancyResponseResponse.EnsureSuccessStatusCode();
            var vacancyResponse = await vacancyResponseResponse.Content.ReadFromJsonAsync<VacancyResponseResponse>();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            if (employer.CompanyId != vacancyResponse.VacancyCompanyId || vacancyResponse.ResponseStatus != VacancyResponseStatusConstants.Waiting)
                return StatusCode((int)HttpStatusCode.Forbidden);

            var acceptVacancyResponseResponse = await httpClient.GetAsync($"{url}/api/VacancyResponse/AcceptVacancyResponse/{vacancyResponseId}");
            acceptVacancyResponseResponse.EnsureSuccessStatusCode();

            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeById/{vacancyResponse.EmployeeId}");
            employeeResponse.EnsureSuccessStatusCode();
            var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();
            var chatResponse = await httpClient.GetAsync($"{url}/api/Chat/GetChat?employerId={employer.Id}&employeeId={vacancyResponse.EmployeeId}");
            if (chatResponse.IsSuccessStatusCode)
            {
                var chat = await chatResponse.Content.ReadFromJsonAsync<ChatResponse>();

                await notificationService.AddNotification(employee.Email,
                    $"Ваш отклик на вакансию <a href=\"vacancy/{vacancyResponse.VacancyId}\">{vacancyResponse.VacancyPosition}</a> был принят");
                await SendJobInvitationMessageAsync(chat.Id, employee.Email, vacancyResponse.Id, vacancyResponse.VacancyPosition);

                return RedirectToAction("GetChatById", "Chat", new { chatId = chat.Id });
            }
            if (chatResponse.StatusCode == HttpStatusCode.NotFound)
            {
                Guid chatId = Guid.NewGuid();
                using StringContent chatJsonContent = new(JsonSerializer.Serialize(new
                {
                    Id = chatId,
                    EmployerId = employer.Id,
                    EmployerFullName = employer.Name + " " + employer.Surname,
                    EmployeeId = employee.Id,
                    EmployeeFullName = employee.Name + " " + employee.Surname
                }), Encoding.UTF8, "application/json");
                var createChatResponse = await httpClient.PostAsync($"{url}/api/Chat/CreateChat", chatJsonContent);
                createChatResponse.EnsureSuccessStatusCode();

                await SendJobInvitationMessageAsync(chatId, employee.Email, vacancyResponse.VacancyId, vacancyResponse.VacancyPosition);

                return RedirectToAction("GetChatById", "Chat", new { chatId });
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [Authorize]
        [HttpPost]
        [Route("vacancy-response/{vacancyResponseId}/reject")]
        public async Task<IActionResult> RejectVacancyResponse(Guid vacancyResponseId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var vacancyResponseResponse = await httpClient.GetAsync($"{url}/api/VacancyResponse/GetVacancyResponseById/{vacancyResponseId}");
            vacancyResponseResponse.EnsureSuccessStatusCode();
            var vacancyResponse = await vacancyResponseResponse.Content.ReadFromJsonAsync<VacancyResponseResponse>();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            if (employer.CompanyId != vacancyResponse.VacancyCompanyId || vacancyResponse.ResponseStatus != VacancyResponseStatusConstants.Waiting)
                return StatusCode((int)HttpStatusCode.Forbidden);

            var rejectVacancyResponseResponse = await httpClient.GetAsync($"{url}/api/VacancyResponse/RejectVacancyResponse/{vacancyResponseId}");
            rejectVacancyResponseResponse.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [HttpGet]
        [Route("company/my-company/interview-invitations")]
        public async Task<IActionResult> GetCompanyInterviewInvitations(string? query, DateTimeOrderByType timeSort = DateTimeOrderByType.Descending,
            int index = 1)
        {
            var encodedQuery = HttpUtility.UrlEncode(query);
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var interviewInvitationsResponse = await httpClient.GetAsync(
                $"{url}/api/InterviewInvitation/GetInterviewInvitationsByCompanyId/{employer.CompanyId}?searchingQuery={encodedQuery}&orderByTimeType={timeSort}&pageNumber={index}");
            interviewInvitationsResponse.EnsureSuccessStatusCode();
            var interviewInvitations = await interviewInvitationsResponse.Content.ReadFromJsonAsync<List<InterviewInvitationResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/InterviewInvitation/DoesNextInterviewInvitationsByCompanyIdPageExist/{employer.CompanyId}?searchingQuery={encodedQuery}&orderByTimeType={timeSort}&currentPageNumber={index}");
            doesNextPageExistResponse.EnsureSuccessStatusCode();
            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.CurrentPageNumber = index;
            ViewBag.DoesNextPageExist = doesNextPageExist;
            ViewBag.TimeSort = timeSort;
            ViewBag.SearchingQuery = query;

            return View(interviewInvitations);
        }

        [Authorize]
        [HttpGet]
        [Route("resumes/{resumeId}/invite-to-interview")]
        public async Task<IActionResult> InviteEmployeeToInterview(Guid resumeId, Guid vacancyId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var vacancyResponse = await httpClient.GetAsync($"{url}/api/Vacancy/GetVacancyById/{vacancyId}");
            vacancyResponse.EnsureSuccessStatusCode();
            var vacancy = await vacancyResponse.Content.ReadFromJsonAsync<VacancyResponse>();

            if (vacancy.CompanyId != employer.CompanyId)
                return StatusCode((int)HttpStatusCode.Forbidden);

            var resumeResponse = await httpClient.GetAsync($"{url}/api/Resume/GetResumeById/{resumeId}");
            resumeResponse.EnsureSuccessStatusCode();
            var resume = await resumeResponse.Content.ReadFromJsonAsync<ResumeResponse>();

            var interviewInvitationResponse = await httpClient.GetAsync(
                $"{url}/api/InterviewInvitation/GetInterviewInvitation?employeeId={resume.EmployeeId}&companyId={vacancy.CompanyId}");
            if (interviewInvitationResponse.IsSuccessStatusCode)
                return View("InvitationToAnInterviewIsAlreadySent");

            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                InvitedCompanyId = vacancy.CompanyId,
                EmployeeId = resume.EmployeeId,
                EmployeeResumeId = resumeId,
                EmployeeName = resume.Name,
                EmployeeSurname = resume.Surname,
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
            var inviteEmployeeResponse = await httpClient.PostAsync($"{url}/api/InterviewInvitation/InviteToInterview", jsonContent);
            inviteEmployeeResponse.EnsureSuccessStatusCode();

            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeById/{resume.EmployeeId}");
            employeeResponse.EnsureSuccessStatusCode();
            var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();
            var chatResponse = await httpClient.GetAsync($"{url}/api/Chat/GetChat?employerId={employer.Id}&employeeId={resume.EmployeeId}");
            if (chatResponse.IsSuccessStatusCode)
            {
                var chat = await chatResponse.Content.ReadFromJsonAsync<ChatResponse>();

                await notificationService.AddNotification(employee.Email,
                    $"Вы были приглашены на собеседование компанией {vacancy.CompanyName} на вакансию <a href=\"vacancy/{vacancy.Id}\">{vacancy.Position}</a>");

                await SendJobInvitationMessageAsync(chat.Id, employee.Email, vacancy.Id, vacancy.Position);

                return RedirectToAction("GetChatById", "Chat", new{ chatId = chat.Id});
            }
            if (chatResponse.StatusCode == HttpStatusCode.NotFound)
            {
                Guid chatId = Guid.NewGuid();
                using StringContent chatJsonContent = new(JsonSerializer.Serialize(new
                {
                    Id = chatId,
                    EmployerId = employer.Id,
                    EmployerFullName = employer.Name + " " + employer.Surname,
                    EmployeeId = employee.Id,
                    EmployeeFullName = employee.Name + " " + employee.Surname
                }), Encoding.UTF8, "application/json");
                var createChatResponse = await httpClient.PostAsync($"{url}/api/Chat/CreateChat", chatJsonContent);
                createChatResponse.EnsureSuccessStatusCode();

                await SendJobInvitationMessageAsync(chatId, employee.Email, vacancy.Id, vacancy.Position);

                return RedirectToAction("GetChatById", "Chat", new { chatId });
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [Authorize]
        [HttpGet]
        [Route("resumes/{resumeId}/invite-to-interview/choose-vacancy")]
        public async Task<IActionResult> ChooseVacancyToInviteToInterview(Guid resumeId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var vacancyResponse = await httpClient.GetAsync($"{url}/api/Vacancy/GetAllVacanciesByCompanyId/{employer.CompanyId}");
            vacancyResponse.EnsureSuccessStatusCode();
            var vacancies = await vacancyResponse.Content.ReadFromJsonAsync<List<VacancyResponse>>();

            if (vacancies.Count == 0)
                return RedirectToAction("AddVacancy","Vacancy");

            if (vacancies.Count == 1)
                return RedirectToAction("InviteEmployeeToInterview", new { resumeId, vacancyId = vacancies[0].Id });

            ViewBag.ResumeId = resumeId;
            return View(vacancies);
        }

        [Authorize]
        [HttpPost]
        [Route("resumes/{resumeId}/invite-to-interview/choose-vacancy")]
        public IActionResult ChooseVacancyToInviteToInterview(Guid resumeId, Guid vacancyId)
        {
            return RedirectToAction("InviteEmployeeToInterview", new {resumeId, vacancyId});
        }

        [Authorize]
        [HttpPost]
        [Route("interviews/{interviewInvitationId}/close")]
        public async Task<IActionResult> CloseInterview(Guid interviewInvitationId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var interviewInvitationResponse = await httpClient.GetAsync(
                $"{url}/api/InterviewInvitation/GetInterviewInvitationById/{interviewInvitationId}");
            interviewInvitationResponse.EnsureSuccessStatusCode();
            var interviewInvitation = await interviewInvitationResponse.Content.ReadFromJsonAsync<InterviewInvitationResponse>();

            var interviewVacancyResponse = await httpClient.GetAsync($"{url}/api/Vacancy/GetVacancyById/{interviewInvitation.VacancyId}");
            interviewVacancyResponse.EnsureSuccessStatusCode();
            var vacancy = await interviewVacancyResponse.Content.ReadFromJsonAsync<VacancyResponse>();

            if (vacancy.CompanyId != employer.CompanyId)
                return StatusCode((int) HttpStatusCode.Forbidden);

            var closeInterviewResponse = await httpClient.GetAsync($"{url}/api/InterviewInvitation/CloseInterview/{interviewInvitationId}");
            closeInterviewResponse.EnsureSuccessStatusCode();

            return RedirectToAction("GetResume","Resume", new {resumeId = interviewInvitation.EmployeeResumeId});
        }

        [Authorize]
        [HttpGet]
        [Route("vacancy/{vacancyId}/interview-invitations")]
        public async Task<IActionResult> GetInterviewInvitationsByVacancyId(Guid vacancyId, string? query,
            DateTimeOrderByType timeSort = DateTimeOrderByType.Descending, int index = 1)
        {
            var encodedQuery = HttpUtility.UrlEncode(query);
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var vacancyResponse = await httpClient.GetAsync($"{url}/api/Vacancy/GetVacancyById/{vacancyId}");
            vacancyResponse.EnsureSuccessStatusCode();
            var vacancy = await vacancyResponse.Content.ReadFromJsonAsync<VacancyResponse>();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
            employerResponse.EnsureSuccessStatusCode();
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            if (vacancy.CompanyId != employer.CompanyId)
                return StatusCode((int)HttpStatusCode.Forbidden);

            var interviewInvitationsResponse = await httpClient.GetAsync(
                    $"{url}/api/InterviewInvitation/GetCompanyInterviewInvitationsByVacancyId/{vacancyId}?searchingQuery={encodedQuery}&orderByTimeType={timeSort}&pageNumber={index}");
            interviewInvitationsResponse.EnsureSuccessStatusCode();
            var interviewInvitations = await interviewInvitationsResponse.Content.ReadFromJsonAsync<List<InterviewInvitationResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/InterviewInvitation/DoesNextCompanyInterviewInvitationsByVacancyIdPageExist/{vacancyId}?searchingQuery={encodedQuery}&orderByTimeType={timeSort}&currentPageNumber={index}");
            doesNextPageExistResponse.EnsureSuccessStatusCode();
            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.DoesNextPageExist = doesNextPageExist;
            ViewBag.SearchingQuery = query;
            ViewBag.TimeSort = timeSort;
            ViewBag.CurrentPageNumber = index;
            ViewBag.VacancyId = vacancyId;
            
            return View(interviewInvitations);
        }

        private async Task SendJobInvitationMessageAsync(Guid chatId, string receiverEmail, Guid jobVacancyId, string jobVacancyPosition)
        {
            string message = $"Приглашаем Вас на собеседование на позицию <a href=\"/vacancy/{jobVacancyId}\">{jobVacancyPosition}</a>";
            string from = User.FindFirst(ClaimTypes.Email).Value;
            await chatHub.Clients.Users(receiverEmail, from).SendAsync("Receive", message, from, DateTime.UtcNow);

            using HttpClient httpClient = httpClientFactory.CreateClient();

            var accountType = User.FindFirst(ClaimTypeConstants.AccountTypeClaimName).Value;
            Guid senderId;
            if (accountType == AccountTypeConstants.Employee)
            {
                var employeeResponse = await httpClient.GetAsync(
                    $"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
                employeeResponse.EnsureSuccessStatusCode();
                var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();
                senderId = employee.Id;
            }
            else
            {
                var employerResponse = await httpClient.GetAsync(
                    $"{url}/api/Employer/GetEmployerByEmail?email={User.Identity.Name}");
                employerResponse.EnsureSuccessStatusCode();
                var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();
                senderId = employer.Id;
            }

            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                SenderId = senderId,
                Text = message
            }), Encoding.UTF8, "application/json");
            var addMessageResponse = await httpClient.PostAsync($"{url}/api/Message/CreateMessage", jsonContent);
            addMessageResponse.EnsureSuccessStatusCode();
        }
    }
}
