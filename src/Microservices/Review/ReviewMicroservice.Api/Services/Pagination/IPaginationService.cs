namespace ReviewMicroservice.Api.Services.Pagination
{
    public interface IPaginationService
    {
        Task<bool> IsNextReviewsByCompanyIdPageExistedAsync(Guid companyId, int currentPageNumber);
        Task<bool> IsNextReviewsByEmployeeIdPageExistedAsync(Guid companyId, int currentPageNumber);
    }
}
