using Microsoft.EntityFrameworkCore;
using ResumeMicroservice.Api.Models;

namespace ResumeMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Resume> Resumes { get; set; }
    }
}
