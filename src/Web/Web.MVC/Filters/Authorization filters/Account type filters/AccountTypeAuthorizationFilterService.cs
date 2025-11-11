using GeneralLibrary.Constants;
using GeneralLibrary.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.MVC.Models.ApiResponses;

namespace Web.MVC.Filters.Authorization_filters.Account_type_filters
{
    public class AccountTypeAuthorizationFilterService(IHttpClientFactory httpClientFactory, IConfiguration configuration, AccountTypeEnum accountType)
        : IAsyncAuthorizationFilter
    {
        private readonly string url = $"{configuration["Url:Protocol"]}://{configuration["Url:Domain"]}";
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string? currentUserEmail = context.HttpContext.User.Identity.Name;
            if (currentUserEmail == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            var currentUserResponse = await httpClient.GetAsync($"{url}/api/User/GetUserByEmail/{currentUserEmail}");
            if (!currentUserResponse.IsSuccessStatusCode)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var currentUser = await currentUserResponse.Content.ReadFromJsonAsync<IdentityUserResponse>();

            switch (accountType)
            {
                case AccountTypeEnum.Employee:
                    if (currentUser.AccountType != AccountTypeConstants.Employee) context.Result = new ForbidResult();
                    break;
                case AccountTypeEnum.Employer:
                    if (currentUser.AccountType != AccountTypeConstants.Employer) context.Result = new ForbidResult();
                    break;
            }
        }
    }
}
