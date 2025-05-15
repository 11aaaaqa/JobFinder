using GeneralLibrary.Constants;
using Microsoft.EntityFrameworkCore;
using ResumeMicroservice.Api.Constants;
using ResumeMicroservice.Api.Database;
using ResumeMicroservice.Api.DTOs;
using ResumeMicroservice.Api.Models;
using ResumeMicroservice.Api.Models.Skills;
using System.Linq;

namespace ResumeMicroservice.Api.Services.Repositories
{
    public class ResumeRepository(ApplicationDbContext context) : IResumeRepository
    {
        public async Task<Resume?> GetResumeByIdAsync(Guid resumeId)
        {
            var resume = await context.Resumes
                .Include(x => x.Educations)
                .Include(x => x.EmployeeExperience)
                .Include(x => x.ForeignLanguages)
                .AsSplitQuery()
                .SingleOrDefaultAsync(x => x.Id == resumeId);
            return resume;
        }

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

            var educations = await context.Education.Where(x => x.ResumeId == model.Id).ToListAsync();
            if (educations.Count == 0 && model.Educations is not null)
            {
                await context.Education.AddRangeAsync(model.Educations);
            }
            else if(educations.Count > 0 && model.Educations is not null)
            {
                var educationsToDelete = educations.Except(model.Educations).ToList();
                if(educationsToDelete.Count > 0)
                    context.Education.RemoveRange(educationsToDelete);

                foreach (var education in educations)
                {
                    var educationNew = model.Educations.SingleOrDefault(x => x.Id == education.Id);
                    if (educationNew is not null)
                    {
                        education.EducationalInstitution = educationNew.EducationalInstitution;
                        education.Specialization = educationNew.Specialization;
                        education.EducationType = educationNew.EducationType;

                        model.Educations.Remove(educationNew);
                    }
                }

                if(model.Educations.Count > 0)
                    await context.Education.AddRangeAsync(model.Educations);
            }
            else
            {
                context.Education.RemoveRange(educations);
            }

            var experiences = await context.EmployeeExperience.Where(x => x.ResumeId == model.Id).ToListAsync();
            if (experiences.Count == 0 && model.EmployeeExperience is not null)
            {
                await context.EmployeeExperience.AddRangeAsync(model.EmployeeExperience);
            }
            else if (experiences.Count > 0 && model.EmployeeExperience is not null)
            {
                var experiencesToDelete = experiences.Except(model.EmployeeExperience).ToList();
                if(experiencesToDelete.Count > 0)
                    context.EmployeeExperience.RemoveRange(experiencesToDelete);

                foreach (var experience in experiences)
                {
                    var experienceNew = model.EmployeeExperience.SingleOrDefault(x => x.Id == experience.Id);
                    if (experienceNew is not null)
                    {
                        experience.CompanyName = experienceNew.CompanyName; experience.CompanyPost = experienceNew.CompanyPost;
                        experience.WorkingFrom = experienceNew.WorkingFrom; experience.WorkingUntil = experienceNew.WorkingUntil;
                        experience.CurrentlyWorkHere = experienceNew.CurrentlyWorkHere;
                        experience.WorkingDuration = experienceNew.WorkingDuration;
                        experience.Responsibilities = experienceNew.Responsibilities;

                        model.EmployeeExperience.Remove(experienceNew);
                    }
                }

                if (model.EmployeeExperience.Count > 0)
                    await context.EmployeeExperience.AddRangeAsync(model.EmployeeExperience);
            }
            else
            {
                context.EmployeeExperience.RemoveRange(experiences);
            }

            var foreignLanguages = await context.ForeignLanguage.Where(x => x.ResumeId == model.Id).ToListAsync();
            if (foreignLanguages.Count == 0 && model.ForeignLanguages is not null)
            {
                await context.ForeignLanguage.AddRangeAsync(model.ForeignLanguages);
            }
            else if (foreignLanguages.Count > 0 && model.ForeignLanguages is not null)
            {
                var languagesToDelete = foreignLanguages.Except(model.ForeignLanguages).ToList();
                if(languagesToDelete.Count > 0)
                    context.ForeignLanguage.RemoveRange(languagesToDelete);

                foreach (var language in foreignLanguages)
                {
                    var languageNew = model.ForeignLanguages.SingleOrDefault(x => x.Id == language.Id);
                    if (languageNew is not null)
                    {
                        language.LanguageName = languageNew.LanguageName;
                        language.LanguageProficiencyLevel = languageNew.LanguageProficiencyLevel;

                        model.ForeignLanguages.Remove(languageNew);
                    }
                }

                if (model.ForeignLanguages.Count > 0)
                    await context.ForeignLanguage.AddRangeAsync(model.ForeignLanguages);
            }
            else
            {
                context.ForeignLanguage.RemoveRange(foreignLanguages);
            }

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
