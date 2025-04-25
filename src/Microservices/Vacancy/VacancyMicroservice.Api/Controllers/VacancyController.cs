using Microsoft.AspNetCore.Mvc;
using VacancyMicroservice.Api.DTOs;
using VacancyMicroservice.Api.Models;
using VacancyMicroservice.Api.Services;
using VacancyMicroservice.Api.Services.Pagination;

namespace VacancyMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacancyController(IVacancyRepository vacancyRepository, IPaginationService paginationService) : ControllerBase
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

        [HttpPost]
        [Route("GetFilteredVacancies")]
        public async Task<IActionResult> GetFilteredVacanciesAsync([FromBody]GetFilteredVacanciesDto model, [FromQuery]int pageNumber)
            => Ok(await vacancyRepository.GetFilteredVacanciesAsync(model, pageNumber));

        [HttpPost]
        [Route("FindFilteredVacancies")]
        public async Task<IActionResult> FindFilteredVacanciesAsync([FromBody] GetFilteredVacanciesDto model,
            [FromQuery]string searchingQuery, [FromQuery]int pageNumber)
            => Ok(await vacancyRepository.SearchFilteredVacanciesAsync(model, searchingQuery, pageNumber));

        [HttpGet]
        [Route("GetVacanciesByCompanyId/{companyId}")]
        public async Task<IActionResult> GetVacanciesByCompanyIdAsync(Guid companyId, int pageNumber, string? searchingQuery)
            => Ok(await vacancyRepository.GetVacanciesByCompanyIdAsync(companyId, pageNumber, searchingQuery));

        [HttpPost]
        [Route("AddVacancy")]
        public async Task<IActionResult> AddVacancyAsync([FromBody] AddVacancyDto model)
        {
            await vacancyRepository.AddVacancyAsync(new Vacancy
            {
                CompanyId = model.CompanyId, CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow), Address = model.Address,
                CompanyName = model.CompanyName, Description = model.Description, EmployerContactEmail = model.EmployerContactEmail,
                EmployerContactPhoneNumber = model.EmployerContactPhoneNumber, EmploymentType = model.EmploymentType, Id = model.Id,
                SalaryTo = model.SalaryTo, SalaryFrom = model.SalaryFrom, WorkExperience = model.WorkExperience, Position = model.Position,
                VacancyCity = model.VacancyCity, RemoteWork = model.RemoteWork, IsArchived = false
            });
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

        [HttpGet]
        [Route("ArchiveVacancy/{vacancyId}")]
        public async Task<IActionResult> ArchiveVacancyAsync(Guid vacancyId)
        {
            await vacancyRepository.ArchiveVacancyAsync(vacancyId);
            return Ok();
        }

        [HttpGet]
        [Route("UnarchiveVacancy/{vacancyId}")]
        public async Task<IActionResult> UnarchiveVacancyAsync(Guid vacancyId)
        {
            await vacancyRepository.UnarchiveVacancyAsync(vacancyId);
            return Ok();
        }

        [HttpGet]
        [Route("GetArchivedVacanciesByCompanyId/{companyId}")]
        public async Task<IActionResult> GetArchivedVacanciesByCompanyIdAsync(Guid companyId, int pageNumber, string? searchingQuery)
            => Ok(await vacancyRepository.GetArchivedVacanciesByCompanyIdAsync(companyId, pageNumber, searchingQuery));

        [HttpGet]
        [Route("DoesNextAllVacanciesPageExist")]
        public async Task<IActionResult> DoesNextAllVacanciesPageExistAsync(int currentPageNumber)
            => Ok(await paginationService.DoesNextAllVacanciesPageExist(currentPageNumber));

        [HttpGet]
        [Route("DoesNextSearchVacanciesPageExist")]
        public async Task<IActionResult> DoesNextSearchVacanciesPageExistAsync(string searchingQuery, int currentPageNumber)
            => Ok(await paginationService.DoesNextSearchVacanciesPageExist(searchingQuery, currentPageNumber));

        [HttpPost]
        [Route("DoesNextFilteredVacanciesPageExist")]
        public async Task<IActionResult> DoesNextFilteredVacanciesPageExistAsync([FromBody] GetFilteredVacanciesDto model, [FromQuery] int currentPageNumber)
            => Ok(await paginationService.DoesNextFilteredVacanciesPageExist(model, currentPageNumber));

        [HttpPost]
        [Route("DoesNextSearchFilteredVacanciesPageExist")]
        public async Task<IActionResult> DoesNextSearchFilteredVacanciesPageExistAsync([FromBody] GetFilteredVacanciesDto model, [FromQuery] string searchingQuery,
            [FromQuery] int currentPageNumber)
            => Ok(await paginationService.DoesNextSearchFilteredVacanciesPageExist(model, searchingQuery, currentPageNumber));

        [HttpGet]
        [Route("DoesNextVacanciesByCompanyIdPageExist/{companyId}")]
        public async Task<IActionResult> DoesNextVacanciesByCompanyIdPageExistAsync(Guid companyId, int currentPageNumber, string? searchingQuery)
            => Ok(await paginationService.DoesNextVacanciesByCompanyIdPageExist(companyId, currentPageNumber, searchingQuery));

        [HttpGet]
        [Route("DoesNextArchivedVacanciesByCompanyIdPageExist/{companyId}")]
        public async Task<IActionResult> DoesNextArchivedVacanciesByCompanyIdPageExistAsync(Guid companyId, int currentPageNumber, string? searchingQuery)
            => Ok(await paginationService.DoesNextArchivedVacanciesByCompanyIdPageExist(companyId, currentPageNumber, searchingQuery));
    }
}
