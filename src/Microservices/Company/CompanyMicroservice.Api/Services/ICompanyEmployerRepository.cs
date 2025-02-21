namespace CompanyMicroservice.Api.Services
{
    public interface ICompanyEmployerRepository
    {
        Task RemoveEmployerFromCompany(Guid companyId, Guid employerId);
    }
}
