namespace CompanyMicroservice.Api.Services.Pagination
{
    public interface ICheckForNextPageExisting
    {
        Task<bool> DoesEmployersRequestedJoiningPageExist(Guid companyId, int pageNumber);
    }
}
