namespace CompanyMicroservice.Api.Services
{
    public interface ICompanyEmployerRepository
    {
        Task RemoveEmployerFromCompanyAsync(Guid companyId, Guid employerId);
        Task RequestJoiningCompanyAsync(Guid companyId, Guid employerId, string employerName, string employerSurname);
    }
}
