using ResumeMicroservice.Api.DTOs;

namespace ResumeMicroservice.Api.Services.Pagination
{
    public interface ICheckForNextPageExistingService
    {
        Task<bool> DoesNextAllResumesPageExistAsync(string? searchingQuery, int currentPageNumber);
        Task<bool> DoesNextResumesWithActiveStatusPageExistAsync(string? searchingQuery, int currentPageNumber);
        Task<bool> DoesNextFilteredResumesPageExistAsync(ResumeFilterModel model, string? searchingQuery, int currentPageNumber);
    }
}
