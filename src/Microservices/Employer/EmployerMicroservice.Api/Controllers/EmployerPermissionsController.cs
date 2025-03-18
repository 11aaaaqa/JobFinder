using EmployerMicroservice.Api.DTOs.Permissions_dtos;
using EmployerMicroservice.Api.Exceptions;
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
            List<string> permissions;
            try
            {
                permissions = await companyPermissionsService.GetAllPermissionsAsync(employerId);
            }
            catch (EmployerHasntPermissionsException e)
            {
                return NotFound();
            }

            return Ok(permissions);
        }

        [HttpGet]
        [Route("CheckForPermission/{employerId}")]
        public async Task<IActionResult> CheckForPermissionAsync(Guid employerId, string permissionName)
        {
            bool doesEmployerHavePermission;
            try
            {
                doesEmployerHavePermission = await companyPermissionsService.CheckForPermissionAsync(employerId, permissionName);
            }
            catch (EmployerHasntPermissionsException e)
            {
                return NotFound();
            }
            return Ok(doesEmployerHavePermission);
        }

        [HttpPost]
        [Route("AddPermissions")]
        public async Task<IActionResult> AddPermissionsAsync([FromBody] AddPermissionsDto model)
        {
            await companyPermissionsService.AddPermissions(model.EmployerId, model.Permissions);
            return Ok();
        }
    }
}
