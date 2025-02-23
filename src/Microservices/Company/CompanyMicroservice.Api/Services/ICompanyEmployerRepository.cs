namespace CompanyMicroservice.Api.Services
{
    public interface ICompanyEmployerRepository
    {
        Task RemoveEmployerFromCompanyAsync(Guid companyId, Guid employerId);
        Task RequestJoiningCompanyAsync(Guid companyId, Guid employerId, string employerName, string employerSurname);
        Task<bool> DidEmployerAlreadyRequestJoiningAsync(Guid employerId, Guid companyId);
    }
}
