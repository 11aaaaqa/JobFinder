using GeneralLibrary.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Web.MVC.Filters.Authorization_filters.Account_type_filters
{
    public class AccountTypeAuthorizationFilter : TypeFilterAttribute
    {
        public AccountTypeAuthorizationFilter(AccountTypeEnum accountType) : base(typeof(AccountTypeAuthorizationFilterService))
        {
            Arguments = new object[] { accountType };
        }
    }
}
