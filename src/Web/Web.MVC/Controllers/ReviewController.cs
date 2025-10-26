using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.DTOs.Review;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.Company;
using Web.MVC.Models.ApiResponses.Review;
using Web.MVC.Models.View_models;

namespace Web.MVC.Controllers
{
    public class ReviewController : Controller
    {
        private readonly string url;
        private readonly IHttpClientFactory httpClientFactory;
        public ReviewController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        }

        [Authorize]
        [HttpGet]
        [Route("reviews/{companyId}/add")]
        public async Task<IActionResult> AddReview(Guid companyId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();
            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            employeeResponse.EnsureSuccessStatusCode();
            var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

            var reviewsByEmployeeResponse = await httpClient.GetAsync($"{url}/api/Review/GetAllReviewsByEmployeeId/{employee.Id}");
            reviewsByEmployeeResponse.EnsureSuccessStatusCode();
            var reviewsByEmployee = await reviewsByEmployeeResponse.Content.ReadFromJsonAsync<List<ReviewResponse>>();

            if (reviewsByEmployee.Any(x => x.CompanyId == companyId))
                return RedirectToAction("ReviewOfTheCurrentCompanyAlreadyExistsInfoPage", "Information", new { companyId});

            var companyResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyId/{companyId}");
            companyResponse.EnsureSuccessStatusCode();
            var company = await companyResponse.Content.ReadFromJsonAsync<CompanyResponse>();
            ViewBag.CompanyName = company.CompanyName;

            return View(new AddReviewDto{Id = Guid.NewGuid(), CompanyId = companyId, EmployeeId = employee.Id});
        }

        [Authorize]
        [HttpPost]
        [Route("reviews/{companyId}/add")]
        public async Task<IActionResult> AddReview(AddReviewDto model)
        {
            if (ModelState.IsValid)
            {
                HttpClient httpClient = httpClientFactory.CreateClient();
                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                var addReviewResponse = await httpClient.PostAsync($"{url}/api/Review/AddReview", jsonContent);
                addReviewResponse.EnsureSuccessStatusCode();
                return RedirectToAction("GetReviewsByCompanyId", new{ companyId = model.CompanyId});
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [Route("reviews/{reviewId}/remove")]
        public async Task<IActionResult> DeleteReview(Guid reviewId, string returnUrl)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();
            
            var employeeResponse = await httpClient.GetAsync($"{url}/api/Employee/GetEmployeeByEmail?email={User.Identity.Name}");
            employeeResponse.EnsureSuccessStatusCode();
            var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

            var reviewResponse = await httpClient.GetAsync($"{url}/api/Review/GetReviewById/{reviewId}");
            reviewResponse.EnsureSuccessStatusCode();
            var review = await reviewResponse.Content.ReadFromJsonAsync<ReviewResponse>();

            if (employee.Id != review.EmployeeId)
                return RedirectToAction("AccessForbidden", "Information");

            var removeReviewResponse = await httpClient.DeleteAsync($"{url}/api/Review/RemoveReview/{reviewId}");
            removeReviewResponse.EnsureSuccessStatusCode();

            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        [Route("companies/{companyId}/reviews")]
        public async Task<IActionResult> GetReviewsByCompanyId(Guid companyId, int index = 1)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();
            var reviewsResponse = await httpClient.GetAsync($"{url}/api/Review/GetReviewsByCompanyIdPagination/{companyId}?pageNumber={index}");
            reviewsResponse.EnsureSuccessStatusCode();
            var reviews = await reviewsResponse.Content.ReadFromJsonAsync<List<ReviewResponse>>();

            var isNextPageExistedResponse = await httpClient.GetAsync(
                $"{url}/api/Review/IsNextReviewsByCompanyIdPageExisted/{companyId}?currentPageNumber={index}");
            isNextPageExistedResponse.EnsureSuccessStatusCode();
            bool isNextPageExisted = await isNextPageExistedResponse.Content.ReadFromJsonAsync<bool>();

            var companyResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyId/{companyId}");
            companyResponse.EnsureSuccessStatusCode();
            var company = await companyResponse.Content.ReadFromJsonAsync<CompanyResponse>();

            return View(new GetReviewsByCompanyId
            {
                CompanyId = companyId,
                GeneralCompanyEstimation = company.Rating,
                CurrentPageNumber = index,
                IsNextPageExisted = isNextPageExisted,
                Reviews = reviews.OrderByDescending(x => x.CreatedAt).ToList()
            });
        }
    }
}