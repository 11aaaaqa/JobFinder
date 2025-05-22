namespace BookmarkMicroservice.Api.Services.Pagination
{
    public interface ICheckForNextPageExistingService
    {
        Task<bool> DoesNextFavoriteVacanciesByEmployeeIdPageExistAsync(Guid employeeId, string? searchingQuery, int currentPageNumber);
    }
}
