using System.ComponentModel;
using CompanyMicroservice.Api.DTOs;
using CompanyMicroservice.Api.Services;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using CompanyMicroservice.Api.Kafka.Producer;
using CompanyMicroservice.Api.Services.Pagination;
using Microsoft.EntityFrameworkCore.Query;

namespace CompanyMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyEmployerController(IKafkaProducer kafkaProducer, ICompanyEmployerRepository companyEmployerRepository,
        ICheckForNextPageExisting checkForNextPage) : ControllerBase
    {
        [HttpGet]
        [Route("GetListOfEmployersRequestedJoining")]
        public async Task<IActionResult> GetListOfEmployersRequestedJoining(Guid companyId, int pageNumber)
            => Ok(await companyEmployerRepository.GetListOfEmployersRequestedJoiningAsync(companyId, pageNumber));

        [HttpGet]
        [Route("DoesNextEmployersRequestedJoiningPageExist")]
        public async Task<IActionResult> DoesNextEmployersRequestedJoiningPageExistAsync(Guid companyId, int pageNumber)
            => Ok(await checkForNextPage.DoesNextEmployersRequestedJoiningPageExist(companyId, pageNumber));

        [HttpPatch]
        [Route("RemoveEmployerFromCompany")]
        public async Task<IActionResult> RemoveEmployerFromCompanyAsync([FromBody] RemoveEmployerFromCompanyDto model)
        {
            await companyEmployerRepository.RemoveEmployerFromCompanyAsync(model.CompanyId, model.EmployerId);

            await kafkaProducer.ProduceAsync("employer-removed-from-company-topic", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(new { model.EmployerId })
            });
            return Ok();
        }

        [HttpPost]
        [Route("RequestJoiningCompany")]
        public async Task<IActionResult> RequestJoiningCompanyAsync([FromBody] RequestJoiningCompanyDto model)
        {
            var joiningRequest = await companyEmployerRepository.DidEmployerAlreadyRequestJoiningAsync(model.EmployerId, model.CompanyId);
            if (joiningRequest)
                return BadRequest();

            await companyEmployerRepository.RequestJoiningCompanyAsync(model.CompanyId, model.EmployerId, model.EmployerName,
                model.EmployerSurname);
            return Ok();
        }

        [HttpGet]
        [Route("AcceptEmployerJoiningRequest")]
        public async Task<IActionResult> AcceptEmployerJoiningRequestAsync(Guid joiningRequestId)
        {
            var request = await companyEmployerRepository.GetJoiningRequestByRequestId(joiningRequestId);
            if(request is null) return BadRequest();

            await kafkaProducer.ProduceAsync("employer-joined-company-topic", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(new { request.EmployerId, request.CompanyId})
            });

            await companyEmployerRepository.DeleteEmployerJoiningAsync(joiningRequestId);
            return Ok();
        }

        [HttpGet]
        [Route("RejectEmployerJoiningRequest")]
        public async Task<IActionResult> RejectEmployerJoiningRequestAsync(Guid joiningRequestId)
        {
            await companyEmployerRepository.DeleteEmployerJoiningAsync(joiningRequestId);
            return Ok();
        }
    }
}
