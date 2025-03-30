using Microsoft.AspNetCore.Mvc;

namespace Web.MVC.Filters.Authorization_filters.Company_filters
{
    public class CompanyPermissionChecker : TypeFilterAttribute
    {
        public CompanyPermissionChecker(string permission) : base(typeof(CompanyPermissionCheckerService))
        {
            Arguments = new object[] { permission };
        }
    }
}
