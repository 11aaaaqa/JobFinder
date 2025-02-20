using CompanyMicroservice.Api.Models;

namespace CompanyMicroservice.Api.Services
{
    public interface IJoinToCompanyRepository
    {
        Task<List<JoiningRequestedEmployer>> GetJoiningRequestedEmployersByCompanyIdAsync(Guid companyId);
        Task AddJoiningRequestAsync(JoiningRequestedEmployer model);
        Task AcceptRequestAsync(Guid requestId);
        Task RejectRequestAsync(Guid requestId);
    }
}
