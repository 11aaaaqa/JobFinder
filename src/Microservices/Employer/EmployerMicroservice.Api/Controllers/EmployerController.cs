using EmployerMicroservice.Api.DTOs;
using EmployerMicroservice.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployerMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployerController(IEmployerRepository employerRepository) : ControllerBase
    {
        [HttpGet]
        [Route("GetEmployerById/{id}")]
        public async Task<IActionResult> GetEmployerByIdAsync(Guid id)
        {
            var employer = await employerRepository.GetEmployerByIdAsync(id);
            if (employer is null) return BadRequest();

            return Ok(employer);
        }

        [HttpGet]
        [Route("GetEmployerByEmail")]
        public async Task<IActionResult> GetEmployerByEmailAsync(string email)
        {
            var employer = await employerRepository.GetEmployerByEmailAsync(email);
            if (employer is null) return BadRequest();

            return Ok(employer);
        }

        [HttpPatch]
        [Route("UpdateEmployer")]
        public async Task<IActionResult> UpdateEmployerAsync([FromBody] UpdateEmployerDto model)
        {
            var succeeded = await employerRepository.UpdateEmployerAsync(model);
            if(!succeeded) return BadRequest();

            return Ok();
        }

        [HttpPatch]
        [Route("AssignCompany")]
        public async Task<IActionResult> AssignCompanyAsync([FromBody]AssignCompanyDto model)
        {
            var succeeded = await employerRepository.AssignCompanyAsync(model.EmployerId, model.CompanyId);
            if(!succeeded) return BadRequest();

            return Ok();
        }
    }
}
