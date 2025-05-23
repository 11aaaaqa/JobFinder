using System.Text.Json;
using CompanyMicroservice.Api.DTOs;
using CompanyMicroservice.Api.Kafka.Producer;
using CompanyMicroservice.Api.Models;
using CompanyMicroservice.Api.Services;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace CompanyMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController(ICompanyRepository companyRepository, IKafkaProducer kafkaProducer,
        ICompanyEmployerRepository companyEmployerRepository) : ControllerBase
    {
        [HttpGet]
        [Route("GetCompanyByCompanyId/{id}")]
        public async Task<IActionResult> GetCompanyByCompanyIdAsync(Guid id)
        {
            var company = await companyRepository.GetCompanyByIdAsync(id);
            if (company is null) return BadRequest();

            return Ok(company);
        }

        [HttpGet]
        [Route("GetCompanyByCompanyName")]
        public async Task<IActionResult> GetCompanyByCompanyNameAsync(string companyName)
        {
            var company = await companyRepository.GetCompanyByCompanyNameAsync(companyName);
            if(company is null) return BadRequest();

            return Ok(company);
        }

        [HttpPatch]
        [Route("UpdateCompany")]
        public async Task<IActionResult> UpdateCompanyAsync([FromBody] UpdateCompanyDto model)
        {
            var company = await companyRepository.GetCompanyByIdAsync(model.Id);
            if(company is null) return BadRequest();
            string oldCompanyName = company.CompanyName;

            var succeeded = await companyRepository.UpdateCompanyAsync(model);
            if (!succeeded) return BadRequest();

            if (oldCompanyName != model.CompanyName)
            {
                await kafkaProducer.ProduceAsync("company-updated-topic", new Message<Null, string>
                {
                    Value = JsonSerializer.Serialize(new
                    {
                        CompanyId = company.Id,
                        NewCompanyName = model.CompanyName,
                        OldCompanyName = oldCompanyName
                    })
                });
            }

            return Ok();
        }

        [HttpDelete]
        [Route("DeleteCompany/{companyId}")]
        public async Task<IActionResult> DeleteCompanyAsync(Guid companyId)
        {
            await companyRepository.DeleteCompanyAsync(companyId);
            await companyEmployerRepository.RemoveAllEmployerRequestsByCompanyIdAsync(companyId);

            await kafkaProducer.ProduceAsync("company-deleted-topic", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(new { CompanyId = companyId })
            });

            return Ok();
        }

        [HttpPost]
        [Route("AddCompany")]
        public async Task<IActionResult> AddCompanyAsync([FromBody] AddCompanyDto model)
        {
            var company = await companyRepository.GetCompanyByCompanyNameAsync(model.CompanyName);
            if (company is not null) return BadRequest();

            var companyId = Guid.NewGuid();
            await companyRepository.AddCompanyAsync(new Company
            {
                Id = companyId, CompanyName = model.CompanyName, CompanyDescription = model.CompanyDescription,
                CompanyColleaguesCount = model.CompanyColleaguesCount, FounderEmployerId = model.FounderEmployerId
            });

            await kafkaProducer.ProduceAsync("company-added-topic", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(new {CompanyId = companyId, EmployerId = model.FounderEmployerId})
            });

            return Ok();
        }
    }
}
