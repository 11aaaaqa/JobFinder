using Microsoft.AspNetCore.Mvc;
using VacancyMicroservice.Api.DTOs;
using VacancyMicroservice.Api.Models;
using VacancyMicroservice.Api.Services;

namespace VacancyMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacancyController(IVacancyRepository vacancyRepository) : ControllerBase
    {
        [HttpGet]
        [Route("GetVacancyById/{vacancyId}")]
        public async Task<IActionResult> GetVacancyByIdAsync(Guid vacancyId)
        {
            var vacancy = await vacancyRepository.GetVacancyByIdAsync(vacancyId);
            if (vacancy is null)
                return BadRequest();
            return Ok(vacancy);
        }

        [HttpGet]
        [Route("GetAllVacancies")]
        public async Task<IActionResult> GetAllVacanciesAsync(int pageNumber)
            => Ok(await vacancyRepository.GetAllVacanciesAsync(pageNumber));

        [HttpGet]
        [Route("FindVacancies")]
        public async Task<IActionResult> FindVacanciesAsync(string searchingQuery, int pageNumber)
            => Ok(await vacancyRepository.SearchVacanciesAsync(searchingQuery, pageNumber));

        [HttpGet]
        [Route("GetFilteredVacancies")]
        public async Task<IActionResult> GetFilteredVacanciesAsync(GetFilteredVacanciesDto model)
            => Ok(await vacancyRepository.GetFilteredVacanciesAsync(model));

        [HttpGet]
        [Route("GetVacanciesByCompanyId/{companyId}")]
        public async Task<IActionResult> GetVacanciesByCompanyIdAsync(Guid companyId, int pageNumber)
            => Ok(await vacancyRepository.GetVacanciesByCompanyIdAsync(companyId, pageNumber));

        [HttpPost]
        [Route("AddVacancy")]
        public async Task<IActionResult> AddVacancyAsync([FromBody] Vacancy vacancy)
        {
            await vacancyRepository.AddVacancyAsync(vacancy);
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteVacancy/{vacancyId}")]
        public async Task<IActionResult> DeleteVacancyAsync(Guid vacancyId)
        {
            await vacancyRepository.DeleteVacancyAsync(vacancyId);
            return Ok();
        }

        [HttpPut]
        [Route("UpdateVacancy")]
        public async Task<IActionResult> UpdateVacancyAsync([FromBody] UpdateVacancyDto model)
        {
            await vacancyRepository.UpdateVacancyAsync(model);
            return Ok();
        }
    }
}
