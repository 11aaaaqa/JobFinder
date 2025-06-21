using System.Text.Json;
using Confluent.Kafka;
using EmployeeMicroservice.Api.DTOs;
using EmployeeMicroservice.Api.Kafka.Kafka_producer;
using EmployeeMicroservice.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController(IEmployeeRepository employeeRepository, IKafkaProducer kafkaProducer) : ControllerBase
    {
        [Route("GetEmployeeById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await employeeRepository.GetEmployeeByIdAsync(id);
            if (employee is null) return BadRequest();

            return Ok(employee);
        }

        [Route("GetEmployeeByEmail")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByEmailAsync(string email)
        {
            var employee = await employeeRepository.GetEmployeeByEmailAsync(email);
            if (employee is null) return BadRequest();

            return Ok(employee);
        }

        [Route("UpdateEmployee")]
        [HttpPatch]
        public async Task<IActionResult> UpdateEmployeeAsync([FromBody] UpdateEmployeeDto model)
        {
            var employee = await employeeRepository.GetEmployeeByIdAsync(model.Id);
            if (employee is null) return BadRequest();

            await employeeRepository.UpdateEmployeeAsync(model);
            await kafkaProducer.ProduceAsync("employee-updated-topic", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(new
                {
                    EmployeeId = model.Id,
                    NewName = model.Name,
                    NewSurname = model.Surname,
                    NewPatronymic = model.Patronymic,
                    NewGender = model.Gender,
                    NewDateOfBirth = model.DateOfBirth
                })
            });

            return Ok();
        }

        [Route("UpdateEmployeeStatus")]
        [HttpPatch]
        public async Task<IActionResult> UpdateEmployeeStatusAsync([FromBody] UpdateEmployeeStatusDto model)
        {
            var employee = await employeeRepository.GetEmployeeByIdAsync(model.EmployeeId);
            if (employee is null) return BadRequest();

            await employeeRepository.UpdateEmployeeStatusAsync(model.EmployeeId, model.Status);

            await kafkaProducer.ProduceAsync("employee-status-updated-topic", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(new {model.EmployeeId, model.Status})
            });

            return Ok();
        }
    }
}
