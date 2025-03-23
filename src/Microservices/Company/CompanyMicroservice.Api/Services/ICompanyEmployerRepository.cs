using CompanyMicroservice.Api.Models;

namespace CompanyMicroservice.Api.Services
{
    public interface ICompanyEmployerRepository
    {
        Task<List<JoiningRequestedEmployer>> GetListOfEmployersRequestedJoiningAsync(Guid companyId, int pageNumber);
        Task RemoveAllEmployerRequestsAsync(Guid employerId);
        Task RequestJoiningCompanyAsync(Guid companyId, Guid employerId, string employerName, string employerSurname);
        Task<bool> DidEmployerAlreadyRequestJoiningAsync(Guid employerId, Guid companyId);
        Task DeleteEmployerJoiningAsync(Guid joiningRequestId);
        Task<JoiningRequestedEmployer?> GetJoiningRequestByRequestId(Guid id);
    }
}
