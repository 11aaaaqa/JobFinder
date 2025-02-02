using EmployeeMicroservice.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController(IEmployeeRepository employeeRepository) : ControllerBase
    {
        [Route("GetEmployeeById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await employeeRepository.GetEmployeeByIdAsync(id);
            if (employee is null) return BadRequest();

            return Ok(employee);
        }

        [Route("GetEmployeeByEmail/{email}")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByEmailAsync(string email)
        {
            var employee = await employeeRepository.GetEmployeeByEmailAsync(email);
            if (employee is null) return BadRequest();

            return Ok(employee);
        }
    }
}
