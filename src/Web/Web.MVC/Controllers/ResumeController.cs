using System.Text;
using System.Text.Json;
using System.Web;
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

            var model = new AddResumeDto
            {
                Id = Guid.NewGuid(), Status = employee.Status, EmployeeId = employee.Id, Name = employee.Name,
                Surname = employee.Surname, Email = employee.Email, DateOfBirth = employee.DateOfBirth, City = employee.City,
                PhoneNumber = employee.PhoneNumber, Patronymic = employee.Patronymic, Gender = employee.Gender
            };

            ViewBag.ResumeId = model.Id;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [Route("employee/profile/resumes/create")]
        public async Task<IActionResult> AddResume(AddResumeDto model, string? returnUrl)
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

                if (returnUrl is not null)
                    return LocalRedirect(returnUrl);

                return RedirectToAction("GetResume", new { resumeId = model.Id});
            }

            ViewBag.ResumeId = model.Id;
            return View(model);
        }

        [HttpGet]
        [Route("resume/{resumeId}")]
        public async Task<IActionResult> GetResume(Guid resumeId, Guid? vacancyResponseId, string? returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var resumeResponse = await httpClient.GetAsync($"{url}/api/Resume/GetResumeById/{resumeId}");
            resumeResponse.EnsureSuccessStatusCode();
            var resume = await resumeResponse.Content.ReadFromJsonAsync<ResumeResponse>();

            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            if (employeeResponse.IsSuccessStatusCode)
            {
                var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();
                ViewBag.IsCurrentUserOwner = resume.EmployeeId == employee.Id;
            }

            if (resume.EmployeeExperience is not null)
            {
                resume.EmployeeExperience = resume.EmployeeExperience
                    .OrderByDescending(x =>
                        DateTime.ParseExact(x.WorkingUntil + "-01", "yyyy-MM-dd", null))
                    .ToList();

                bool experienceUpdateRequired = false;
                foreach (var experience in resume.EmployeeExperience)
                {
                    if (experience.CurrentlyWorkHere)
                    {
                        DateTime workingUntil = DateTime.ParseExact(experience.WorkingUntil + "-01", "yyyy-MM-dd", null);
                        DateTime workingFrom = DateTime.ParseExact(experience.WorkingFrom + "-01", "yyyy-MM-dd", null);
                        DateTime currentDate = DateTime.UtcNow;
                        if (currentDate > workingUntil.AddMonths(1))
                        {
                            var workingUntilNew = new DateTime(currentDate.Year, currentDate.Month, 1);
                            string workingUntilNewString = workingUntilNew.Month < 10
                                ? $"{workingUntilNew.Year}-0{workingUntilNew.Month}"
                                : $"{workingUntilNew.Year}-{workingUntilNew.Month}";

                            experience.WorkingUntil = workingUntilNewString;
                            experience.WorkingDuration = workingUntilNew - workingFrom;
                            experienceUpdateRequired = true;
                        }
                    }
                }

                if (experienceUpdateRequired)
                {
                    using StringContent jsonContent = new(JsonSerializer.Serialize(new
                    {
                        resume.Id, resume.ResumeTitle, resume.OccupationTypes, resume.WorkTypes, resume.Name, resume.Surname,
                        resume.Patronymic, resume.Gender, resume.DateOfBirth, resume.City, resume.ReadyToMove, resume.PhoneNumber,
                        resume.Email, resume.AboutMe, resume.DesiredSalary, resume.Educations, resume.EmployeeExperience, resume.ForeignLanguages
                    }), Encoding.UTF8, "application/json");
                    var updateResumeResponse = await httpClient.PutAsync($"{url}/api/Resume/UpdateResume", jsonContent);
                    updateResumeResponse.EnsureSuccessStatusCode();
                }
            }

            if (vacancyResponseId is not null)
            {
                var vacancyResponseResponse = await httpClient.GetAsync($"{url}/api/VacancyResponse/GetVacancyResponseById/{vacancyResponseId}");
                if (vacancyResponseResponse.IsSuccessStatusCode && returnUrl is not null)
                {
                    ViewBag.VacancyResponseValid = true;
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.VacancyResponseId = vacancyResponseId;
                }
            }

            return View(resume);
        }

        [Authorize]
        [HttpPost]
        [Route("resume/delete/{resumeId}")]
        public async Task<IActionResult> DeleteResume(Guid resumeId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            employeeResponse.EnsureSuccessStatusCode();
            var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

            var resumeResponse = await httpClient.GetAsync($"{url}/api/Resume/GetResumeById/{resumeId}");
            resumeResponse.EnsureSuccessStatusCode();
            var resume = await resumeResponse.Content.ReadFromJsonAsync<ResumeResponse>();

            if (employee.Id != resume.EmployeeId)
                return RedirectToAction("AccessForbidden", "Information");

            var deleteResumeResponse = await httpClient.DeleteAsync($"{url}/api/Resume/DeleteResume/{resumeId}");
            deleteResumeResponse.EnsureSuccessStatusCode();

            return RedirectToAction("GetMyResumes");
        }

        [Authorize]
        [HttpGet]
        [Route("resume/edit/{resumeId}")]
        public async Task<IActionResult> EditResume(Guid resumeId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var resumeResponse = await httpClient.GetAsync($"{url}/api/Resume/GetResumeById/{resumeId}");
            resumeResponse.EnsureSuccessStatusCode();
            var model = await resumeResponse.Content.ReadFromJsonAsync<EditResumeDto>();

            ViewBag.ResumeId = model.Id;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [Route("resume/edit/{resumeId}")]
        public async Task<IActionResult> EditResume(EditResumeDto model)
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

                var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
                employeeResponse.EnsureSuccessStatusCode();
                var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

                if (employee.Id != model.EmployeeId)
                    return RedirectToAction("AccessForbidden", "Information");

                using StringContent jsonContent = new(JsonSerializer.Serialize(new
                {
                    model.Id, model.ResumeTitle, model.OccupationTypes, model.WorkTypes, model.Name, model.Surname, model.Patronymic,
                    model.Gender, model.DateOfBirth, model.City, model.ReadyToMove, model.PhoneNumber, model.Email, model.AboutMe,
                    model.DesiredSalary, model.ForeignLanguages, model.Educations, model.EmployeeExperience
                }), Encoding.UTF8, "application/json");
                var updateResponse = await httpClient.PutAsync($"{url}/api/Resume/UpdateResume", jsonContent);
                updateResponse.EnsureSuccessStatusCode();

                return RedirectToAction("GetResume", new { resumeId = model.Id});
            }

            ViewBag.ResumeId = model.Id;
            return View(model);
        }

        [HttpGet]
        [Route("resumes/all")]
        public async Task<IActionResult> GetAllResumes(string? searchingQuery, int index = 1)
        {
            string? encodedQuery = HttpUtility.UrlEncode(searchingQuery);
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var resumesResponse = await httpClient.GetAsync(
                $"{url}/api/Resume/GetAllResumes?searchingQuery={encodedQuery}&pageNumber={index}");
            resumesResponse.EnsureSuccessStatusCode();

            var resumes = await resumesResponse.Content.ReadFromJsonAsync<List<ResumeResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/Resume/DoesNextAllResumesPageExist?searchingQuery={encodedQuery}&currentPageNumber={index}");
            doesNextPageExistResponse.EnsureSuccessStatusCode();

            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.CurrentPageNumber = index;
            ViewBag.DoesNextPageExist = doesNextPageExist;
            ViewBag.SearchingQuery = searchingQuery;

            return View(resumes);
        }

        [HttpGet]
        [Route("resumes")]
        public async Task<IActionResult> GetResumesWithActiveStatus(string? searchingQuery, ResumeFilterDto? model, int index = 1)
        {
            if (model.Cities is null && model.DesiredSalaryTo is null && model.OccupationTypes is null &&
                model.ResumeTitle is null && model.WorkTypes is null && model.WorkExperience is null)
            {
                model = null;
            }

            string? encodedQuery = HttpUtility.UrlEncode(searchingQuery);
            using HttpClient httpClient = httpClientFactory.CreateClient();

            List<ResumeResponse> resumes = new();
            bool doesNextPageExist;
            if (model is null)
            {
                var resumesResponse = await httpClient.GetAsync(
                    $"{url}/api/Resume/GetResumesWithActiveStatus?searchingQuery={encodedQuery}&pageNumber={index}");
                resumesResponse.EnsureSuccessStatusCode();

                resumes = await resumesResponse.Content.ReadFromJsonAsync<List<ResumeResponse>>();

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"{url}/api/Resume/DoesNextResumesWithActiveStatusPageExist?searchingQuery={encodedQuery}&currentPageNumber={index}");
                doesNextPageExistResponse.EnsureSuccessStatusCode();

                doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            }
            else
            {
                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var resumesResponse = await httpClient.PostAsync(
                    $"{url}/api/Resume/GetFilteredResumes?searchingQuery={encodedQuery}&pageNumber={index}", jsonContent);
                resumesResponse.EnsureSuccessStatusCode();

                resumes = await resumesResponse.Content.ReadFromJsonAsync<List<ResumeResponse>>();

                var doesNextPageExistResponse = await httpClient.PostAsync(
                    $"{url}/api/Resume/DoesNextFilteredResumesPageExist?searchingQuery={encodedQuery}&currentPageNumber={index}", jsonContent);
                doesNextPageExistResponse.EnsureSuccessStatusCode();

                doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            }

            ViewBag.Filter = model;
            ViewBag.CurrentPageNumber = index;
            ViewBag.DoesNextPageExist = doesNextPageExist;
            ViewBag.SearchingQuery = searchingQuery;

            return View(resumes);
        }

        [HttpGet]
        [Route("resumes/filter")]
        public IActionResult SetResumeFilter(string? resumeTitle, List<string>? occupationTypes, List<string>? workTypes, List<string>? cities,
            uint? desiredSalaryTo, string? workExperience)
        {
            occupationTypes ??= new List<string>();
            cities ??= new List<string>();
            workTypes ??= new List<string>();
            return View(new ResumeFilterDto
            {
                OccupationTypes = occupationTypes, WorkTypes = workTypes, Cities = cities, DesiredSalaryTo = desiredSalaryTo,
                ResumeTitle = resumeTitle, WorkExperience = workExperience
            });
        }

        [HttpPost]
        [Route("resumes/filter")]
        public IActionResult SetResumeFilter(ResumeFilterDto model)
        {
            if (model.OccupationTypes is not null && model.OccupationTypes.Count == 0)
                model.OccupationTypes = null;
            if (model.Cities is not null && model.Cities.Count == 0)
                model.Cities = null;
            if (model.WorkTypes is not null && model.WorkTypes.Count == 0)
                model.WorkTypes = null;
            if (ModelState.IsValid)
            {
                return RedirectToAction("GetResumesWithActiveStatus", new
                {
                    model.OccupationTypes, model.WorkTypes, model.Cities, 
                    model.DesiredSalaryTo, model.ResumeTitle, model.WorkExperience
                });
            }
            return View(model);
        }
    }
}
