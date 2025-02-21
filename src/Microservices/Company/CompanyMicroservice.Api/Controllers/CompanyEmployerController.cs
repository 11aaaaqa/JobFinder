using CompanyMicroservice.Api.DTOs;
using CompanyMicroservice.Api.Services;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using CompanyMicroservice.Api.Kafka.Producer;

namespace CompanyMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyEmployerController(IKafkaProducer kafkaProducer, ICompanyEmployerRepository companyEmployerRepository) : ControllerBase
    {
        [HttpPatch]
        [Route("RemoveEmployerFromCompany")]
        public async Task<IActionResult> RemoveEmployerFromCompanyAsync([FromBody] RemoveEmployerFromCompanyDto model)
        {
            await companyEmployerRepository.RemoveEmployerFromCompany(model.CompanyId, model.EmployerId);

            await kafkaProducer.ProduceAsync("employer-removed-from-company-topic", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(new { model.EmployerId })
            });
            return Ok();
        }
    }
}
