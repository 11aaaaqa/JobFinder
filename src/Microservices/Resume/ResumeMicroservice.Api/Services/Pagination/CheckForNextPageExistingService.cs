using GeneralLibrary.Constants;
using Microsoft.EntityFrameworkCore;
using ResumeMicroservice.Api.Constants;
using ResumeMicroservice.Api.Database;
using ResumeMicroservice.Api.DTOs;

namespace ResumeMicroservice.Api.Services.Pagination
{
    public class CheckForNextPageExistingService(ApplicationDbContext context) : ICheckForNextPageExistingService
    {
        public async Task<bool> DoesNextAllResumesPageExistAsync(string? searchingQuery, int currentPageNumber)
        {
            var resumes = context.Resumes.AsQueryable();
            if (searchingQuery is not null)
                resumes = resumes.Where(x => x.ResumeTitle.ToLower().Contains(searchingQuery.ToLower()));

            return await resumes.Skip(currentPageNumber * PaginationConstants.ResumePageSize).CountAsync() > 0;
        }

        public async Task<bool> DoesNextResumesWithActiveStatusPageExistAsync(string? searchingQuery, int currentPageNumber)
        {
            var resumes = context.Resumes.Where(
                x => x.Status == WorkStatusConstants.LookingForJob | x.Status == WorkStatusConstants.ConsideringOffers).AsQueryable();
            if (searchingQuery is not null)
                resumes = resumes.Where(x => x.ResumeTitle.ToLower().Contains(searchingQuery.ToLower()));

            return await resumes.Skip(currentPageNumber * PaginationConstants.ResumePageSize).CountAsync() > 0;
        }

        public async Task<bool> DoesNextFilteredResumesPageExistAsync(ResumeFilterModel model, string? searchingQuery, int currentPageNumber)
        {
            var resumes = context.Resumes.Where(
                x => x.Status == WorkStatusConstants.LookingForJob | x.Status == WorkStatusConstants.ConsideringOffers).AsQueryable();

            if (model.ResumeTitle is not null)
                resumes = resumes.Where(x => x.ResumeTitle.ToLower().Contains(model.ResumeTitle.ToLower()));
            if (model.Cities is not null)
                resumes = resumes.Where(x => model.Cities.Contains(x.City));
            if (model.OccupationTypes is not null)
                resumes = resumes.Where(x => model.OccupationTypes.Intersect(x.OccupationTypes).Any());
            if (model.WorkTypes is not null)
                resumes = resumes.Where(x => model.WorkTypes.Intersect(x.WorkTypes).Any());
            if (model.DesiredSalaryTo is not null)
                resumes = resumes.Where(x => x.DesiredSalary <= model.DesiredSalaryTo);
            if (model.WorkingExperienceFrom is not null)
                resumes = resumes.Where(x => x.WorkingExperience >= model.WorkingExperienceFrom);

            if (searchingQuery is not null)
                resumes = resumes.Where(x => x.ResumeTitle.ToLower().Contains(searchingQuery.ToLower()));

            return await resumes.Skip(currentPageNumber * PaginationConstants.ResumePageSize).CountAsync() > 0;
        }
    }
}
