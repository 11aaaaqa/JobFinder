using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.MVC.Models.ApiResponses.Company;
using Web.MVC.Models.ApiResponses.Employer;

namespace Web.MVC.Filters.Authorization_filters.Company_filters
{
    public class CompanyPermissionCheckerService(IHttpClientFactory httpClientFactory, IConfiguration configuration, string permission)
        : IAsyncAuthorizationFilter
    {
        private readonly string url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
        {
            var currentUserEmail = filterContext.HttpContext.User.Identity.Name;
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var employerResponse = await httpClient.GetAsync($"{url}/api/Employer/GetEmployerByEmail?email={currentUserEmail}");
            if (!employerResponse.IsSuccessStatusCode)
            {
                filterContext.Result = new UnauthorizedResult();
                return;
            }
            var employer = await employerResponse.Content.ReadFromJsonAsync<EmployerResponse>();

            var companyResponse = await httpClient.GetAsync($"{url}/api/Company/GetCompanyByCompanyId/{employer.CompanyId}");
            if (!companyResponse.IsSuccessStatusCode)
            {
                filterContext.Result = new UnauthorizedResult();
                return;
            }
            var company = await companyResponse.Content.ReadFromJsonAsync<CompanyResponse>();
            if (employer.Id == company.FounderEmployerId)
                return;

            var response = await httpClient.GetAsync(
                $"{url}/api/EmployerPermissions/CheckForPermission/{employer.Id}?permissionName={permission}");
            if (!response.IsSuccessStatusCode)
            {
                filterContext.Result = new UnauthorizedResult();
                return;
            }
            bool employerHasPermission = await response.Content.ReadFromJsonAsync<bool>();

            if (!employerHasPermission)
                filterContext.Result = new ForbidResult();
        }
    }
}
