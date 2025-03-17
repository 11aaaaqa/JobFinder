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
        [Route("GetEmployerPermissions/{employerId}")]
        public async Task<IActionResult> GetEmployerPermissionsAsync(Guid employerId)
            => Ok(await companyPermissionsService.GetAllPermissionsAsync(employerId));

        [HttpGet]
        [Route("CheckForPermission/{employerId}")]
        public async Task<IActionResult> CheckForPermissionAsync(Guid employerId, string permissionName)
            => Ok(await companyPermissionsService.CheckForPermissionAsync(employerId, permissionName));

        [HttpPost]
        [Route("AddPermissions")]
        public async Task<IActionResult> AddPermissionsAsync([FromBody] AddPermissionsDto model)
        {
            await companyPermissionsService.AddPermissions(model.EmployerId, model.Permissions);
            return Ok();
        }
    }
}
