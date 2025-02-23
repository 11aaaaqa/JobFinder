namespace CompanyMicroservice.Api.Services.Pagination
{
    public interface ICheckForNextPageExisting
    {
        Task<bool> DoesNextEmployersRequestedJoiningPageExist(Guid companyId, int pageNumber);
    }
}
