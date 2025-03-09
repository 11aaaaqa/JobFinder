namespace EmployerMicroservice.Api.Services.Pagination
{
    public interface IPaginationService
    {
        Task<bool> DoesNextEmployersByCompanyPageExist(Guid companyId, int currentPageNumber);
        Task<bool> DoesNextSearchingEmployersPageExist(Guid companyId, int currentPageNumber, string searchingQuery);
    }
}
