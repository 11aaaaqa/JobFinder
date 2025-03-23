using CompanyMicroservice.Api.DTOs;
using CompanyMicroservice.Api.Services;
using Microsoft.AspNetCore.Mvc;
using CompanyMicroservice.Api.Services.Pagination;

namespace CompanyMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyEmployerController(ICompanyEmployerRepository companyEmployerRepository, 
        ICheckForNextPageExisting checkForNextPage) : ControllerBase
    {
        [HttpGet]
        [Route("GetListOfEmployersRequestedJoining/{companyId}")]
        public async Task<IActionResult> GetListOfEmployersRequestedJoining(Guid companyId, int pageNumber)
            => Ok(await companyEmployerRepository.GetListOfEmployersRequestedJoiningAsync(companyId, pageNumber));

        [HttpGet]
        [Route("DoesEmployersRequestedJoiningPageExist/{companyId}")]
        public async Task<IActionResult> DoesEmployersRequestedJoiningPageExistAsync(Guid companyId, int pageNumber)
            => Ok(await checkForNextPage.DoesEmployersRequestedJoiningPageExist(companyId, pageNumber));

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
        [Route("RejectEmployerJoiningRequest/{joiningRequestId}")]
        public async Task<IActionResult> RejectEmployerJoiningRequestAsync(Guid joiningRequestId)
        {
            await companyEmployerRepository.DeleteEmployerJoiningAsync(joiningRequestId);
            return Ok();
        }

        [HttpGet]
        [Route("DidEmployerRequestJoining")]
        public async Task<IActionResult> DidEmployerRequestJoiningAsync(Guid employerId, Guid companyId)
            => Ok(await companyEmployerRepository.DidEmployerAlreadyRequestJoiningAsync(employerId, companyId));

        [HttpGet]
        [Route("GetJoiningRequestByRequestId/{requestId}")]
        public async Task<IActionResult> GetJoiningRequestByRequestIdAsync(Guid requestId)
        {
            var request = await companyEmployerRepository.GetJoiningRequestByRequestId(requestId);
            if (request is null)
                return NotFound();
            return Ok(request);
        }

        [HttpGet]
        [Route("RemoveAllEmployerRequestsByEmployerId/{employerId}")]
        public async Task<IActionResult> RemoveAllEmployerRequestsAsync(Guid employerId)
        {
            await companyEmployerRepository.RemoveAllEmployerRequestsAsync(employerId);
            return Ok();
        }
    }
}
