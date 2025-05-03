using GeneralLibrary.Constants;
using Microsoft.EntityFrameworkCore;
using ResumeMicroservice.Api.Constants;
using ResumeMicroservice.Api.Database;
using ResumeMicroservice.Api.DTOs;
using ResumeMicroservice.Api.Models;

namespace ResumeMicroservice.Api.Services.Repositories
{
    public class ResumeRepository(ApplicationDbContext context) : IResumeRepository
    {
        public async Task<Resume?> GetResumeByIdAsync(Guid resumeId)
            => await context.Resumes.SingleOrDefaultAsync(x => x.Id == resumeId);

        public async Task<List<Resume>> GetAllResumesAsync(string? searchingQuery, int pageNumber)
        {
            var resumes = context.Resumes.AsQueryable();
            if (searchingQuery is not null)
                resumes = resumes.Where(x => x.ResumeTitle.ToLower().Contains(searchingQuery.ToLower()));

            return await resumes.Skip((pageNumber - 1) * PaginationConstants.ResumePageSize)
                .Take(PaginationConstants.ResumePageSize).ToListAsync();
        }

        public async Task<List<Resume>> GetResumesWithActiveStatusAsync(string? searchingQuery, int pageNumber)
        {
            var resumes = context.Resumes.Where(
                x => x.Status == WorkStatusConstants.LookingForJob | x.Status == WorkStatusConstants.ConsideringOffers).AsQueryable();
            if(searchingQuery is not null)
                resumes = resumes.Where(x => x.ResumeTitle.ToLower().Contains(searchingQuery.ToLower()));

            return await resumes.Skip((pageNumber - 1) * PaginationConstants.ResumePageSize)
                .Take(PaginationConstants.ResumePageSize).ToListAsync();
        }

        public async Task<List<Resume>> GetFilteredResumesAsync(ResumeFilterModel model, string? searchingQuery, int pageNumber)
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

            return await resumes.Skip((pageNumber - 1) * PaginationConstants.ResumePageSize)
                .Take(PaginationConstants.ResumePageSize).ToListAsync();
        }

        public async Task<List<Resume>> GetResumesByEmployeeIdAsync(Guid employeeId)
            => await context.Resumes.Where(x => x.EmployeeId == employeeId).ToListAsync();

        public async Task AddResumeAsync(Resume model)
        {
            await context.Resumes.AddAsync(model);
            await context.SaveChangesAsync();
        }

        public async Task UpdateResumeAsync(UpdateResumeDto model)
        {
            var resume = await context.Resumes.SingleOrDefaultAsync(x => x.Id == model.Id);
            if (resume is null)
                return;

            resume.ResumeTitle = model.ResumeTitle; resume.Name = model.Name; resume.OccupationTypes = model.OccupationTypes;
            resume.WorkTypes = model.WorkTypes; resume.Surname = model.Surname; resume.Patronymic = model.Patronymic;
            resume.Gender = model.Gender; resume.DateOfBirth = model.DateOfBirth; resume.ReadyToMove = model.ReadyToMove;
            resume.PhoneNumber = model.PhoneNumber; resume.Email = model.Email; resume.AboutMe = model.AboutMe;
            resume.DesiredSalary = model.DesiredSalary; resume.WorkingExperience = model.WorkingExperience;
            resume.Educations = model.Educations; resume.ForeignLanguages = model.ForeignLanguages;
            resume.EmployeeExperience = model.EmployeeExperience;

            await context.SaveChangesAsync();
        }

        public async Task DeleteResumeAsync(Guid resumeId)
        {
            var resume = await context.Resumes.SingleOrDefaultAsync(x => x.Id == resumeId);
            if (resume is null)
                return;

            context.Resumes.Remove(resume);
            await context.SaveChangesAsync();
        }
    }
}
