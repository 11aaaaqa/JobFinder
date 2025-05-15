using Microsoft.EntityFrameworkCore;
using ResumeMicroservice.Api.Models;
using ResumeMicroservice.Api.Models.Skills;

namespace ResumeMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<ForeignLanguage> ForeignLanguage { get; set; }
        public DbSet<EmployeeExperience> EmployeeExperience { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resume>()
                .HasMany(x => x.EmployeeExperience)
                .WithOne()
                .HasForeignKey(x => x.ResumeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>()
                .HasMany(x => x.ForeignLanguages)
                .WithOne()
                .HasForeignKey(x => x.ResumeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>()
                .HasMany(x => x.Educations)
                .WithOne()
                .HasForeignKey(x => x.ResumeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
