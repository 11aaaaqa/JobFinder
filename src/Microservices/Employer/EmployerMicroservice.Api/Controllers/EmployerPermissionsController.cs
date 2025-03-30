using EmployerMicroservice.Api.DTOs.Permissions_dtos;
using EmployerMicroservice.Api.Services.Company_permissions_services;
using Microsoft.AspNetCore.Mvc;

namespace EmployerMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployerPermissionsController(IEmployerPermissionsService companyPermissionsService) : ControllerBase
    {
        [HttpGet]
        [Route("GetEmployerPermissions/{employerId}")]
        public async Task<IActionResult> GetEmployerPermissionsAsync(Guid employerId)
        {
            var permissions = await companyPermissionsService.GetAllPermissionsAsync(employerId);

            return Ok(permissions);
        }

        [HttpGet]
        [Route("CheckForPermission/{employerId}")]
        public async Task<IActionResult> CheckForPermissionAsync(Guid employerId, string permissionName)
        {
            bool doesEmployerHavePermission = await companyPermissionsService.CheckForPermissionAsync(employerId, permissionName);
            return Ok(doesEmployerHavePermission);
        }

        [HttpPost]
        [Route("AddPermissions")]
        public async Task<IActionResult> AddPermissionsAsync([FromBody] AddPermissionsDto model)
        {
            await companyPermissionsService.AddPermissions(model.EmployerId, model.Permissions);
            return Ok();
        }

        [HttpGet]
        [Route("RemoveAllEmployerPermissions/{employerId}")]
        public async Task<IActionResult> RemoveAllEmployerPermissionsAsync(Guid employerId)
        {
            await companyPermissionsService.RemoveAllEmployerPermissions(employerId);
            return Ok();
        }
    }
}
