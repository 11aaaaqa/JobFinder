using CompanyMicroservice.Api.DTOs;
using CompanyMicroservice.Api.Models;
using CompanyMicroservice.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CompanyMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController(ICompanyRepository companyRepository) : ControllerBase
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
        [Route("GetCompanyByCompanyName/{companyName}")]
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
            var succeeded = await companyRepository.UpdateCompanyAsync(model);
            if (!succeeded) return BadRequest();

            return Ok();
        }

        [HttpDelete]
        [Route("DeleteCompany")]
        public async Task<IActionResult> DeleteCompanyAsync([FromBody] DeleteCompanyDto model)
        {
            await companyRepository.DeleteCompanyAsync(model.CompanyId);

            return Ok();
        }

        [HttpPost]
        [Route("AddCompany")]
        public async Task<IActionResult> AddCompanyAsync([FromBody] AddCompanyDto model)
        {
            var company = await companyRepository.GetCompanyByCompanyNameAsync(model.CompanyName);
            if (company is not null) return BadRequest();

            await companyRepository.AddCompanyAsync(new Company
            {
                Id = Guid.NewGuid(), CompanyName = model.CompanyName, CompanyDescription = model.CompanyDescription,
                CompanyColleaguesCount = model.CompanyColleaguesCount, FounderEmployerId = model.FounderEmployerId
            });

            //добавить CompanyId в модель Employer

            return Ok();
        }
    }
}
