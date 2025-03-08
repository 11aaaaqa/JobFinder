namespace EmployerMicroservice.Api.Services.Pagination
{
    public interface IPaginationService
    {
        Task<bool> DoesNextEmployersByCompanyPageExist(Guid companyId, int currentPageNumber);
    }
}
