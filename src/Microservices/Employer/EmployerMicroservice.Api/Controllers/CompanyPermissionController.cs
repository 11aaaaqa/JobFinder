using EmployerMicroservice.Api.DTOs.Permissions_dtos;
using EmployerMicroservice.Api.Services.Company_permissions_services;
using Microsoft.AspNetCore.Mvc;

namespace EmployerMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyPermissionController(ICompanyPermissionsService companyPermissionsService) : ControllerBase
    {
        [HttpGet]
        [Route("GetEmployerPermissions/{companyId}")]
        public async Task<IActionResult> GetEmployerPermissionsAsync(Guid employerId, Guid companyId)
            => Ok(await companyPermissionsService.GetAllPermissionsAsync(employerId, companyId));

        [HttpGet]
        [Route("CheckForPermission/{companyId}")]
        public async Task<IActionResult> CheckForPermissionAsync(Guid employerId, Guid companyId, string permissionName)
            => Ok(await companyPermissionsService.CheckForPermissionAsync(employerId, companyId, permissionName));

        [HttpPost]
        [Route("AddPermissions")]
        public async Task<IActionResult> AddPermissionsAsync([FromBody] AddPermissionsDto model)
        {
            await companyPermissionsService.AddPermissions(model.EmployerId, model.CompanyId, model.Permissions);
            return Ok();
        }
    }
}
