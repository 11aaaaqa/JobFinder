using ResumeMicroservice.Api.DTOs;
using ResumeMicroservice.Api.Models;

namespace ResumeMicroservice.Api.Services.Repositories
{
    public interface IResumeRepository
    {
        Task<List<Resume>> GetAllResumesAsync(string? searchingQuery, int pageNumber);
        Task<List<Resume>> GetResumesWithActiveStatusAsync(string? searchingQuery, int pageNumber);
        Task<List<Resume>> GetFilteredResumesAsync(ResumeFilterModel model, string? searchingQuery, int pageNumber);
        Task<List<Resume>> GetResumesByEmployeeIdAsync(Guid employeeId);
        Task AddResumeAsync(Resume model);
        Task UpdateResumeAsync(UpdateResumeDto model);
        Task DeleteResumeAsync(Guid resumeId);
    }
}
