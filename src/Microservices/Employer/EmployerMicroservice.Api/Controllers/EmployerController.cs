using System.Text.Json;
using Confluent.Kafka;
using EmployerMicroservice.Api.DTOs;
using EmployerMicroservice.Api.Kafka.Producer;
using EmployerMicroservice.Api.Services;
using EmployerMicroservice.Api.Services.Company_permissions_services;
using EmployerMicroservice.Api.Services.Pagination;
using EmployerMicroservice.Api.Services.Searching_services;
using Microsoft.AspNetCore.Mvc;

namespace EmployerMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployerController(IEmployerRepository employerRepository, IPaginationService paginationService,
        ISearchingService searchingService, IEmployerPermissionsService employerPermissionsService, IKafkaProducer kafkaProducer) : ControllerBase
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

            await kafkaProducer.ProduceAsync("employer-updated-topic", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(new
                {
                    EmployerId = model.Id,
                    NewName = model.Name,
                    NewSurname = model.Surname
                })
            });

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

        [HttpGet]
        [Route("GetEmployersByCompanyId/{companyId}")]
        public async Task<IActionResult> GetEmployersByCompanyIdAsync(Guid companyId, int pageNumber)
            => Ok(await employerRepository.GetEmployersByCompanyId(companyId, pageNumber));

        [HttpGet]
        [Route("DoesNextEmployersByCompanyIdPageExist/{companyId}")]
        public async Task<IActionResult> DoesNextEmployersByCompanyIdPageExistAsync(Guid companyId, int currentPageNumber)
            => Ok(await paginationService.DoesNextEmployersByCompanyPageExist(companyId, currentPageNumber));

        [HttpGet]
        [Route("FindEmployers/{companyId}")]
        public async Task<IActionResult> FindEmployersAsync(Guid companyId, int pageNumber, string searchingQuery)
            => Ok(await searchingService.FindEmployersAsync(companyId, pageNumber, searchingQuery));

        [HttpGet]
        [Route("DoesNextFindEmployersPageExist/{companyId}")]
        public async Task<IActionResult> DoesNextFindEmployersPageExistAsync(Guid companyId, int currentPageNumber, string searchingQuery)
            => Ok(await paginationService.DoesNextSearchingEmployersPageExist(companyId, currentPageNumber, searchingQuery));

        [HttpGet]
        [Route("RemoveEmployerFromCompany/{employerId}")]
        public async Task<IActionResult> RemoveEmployerFromCompanyAsync(Guid employerId)
        {
            bool isRemoved = await employerRepository.RemoveEmployerFromCompanyAsync(employerId);
            if (!isRemoved)
                return BadRequest();
            await employerPermissionsService.RemoveAllEmployerPermissions(employerId);
            return Ok();
        }
    }
}
