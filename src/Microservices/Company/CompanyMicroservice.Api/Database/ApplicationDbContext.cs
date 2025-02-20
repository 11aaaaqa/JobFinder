using CompanyMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<JoiningRequestedEmployer> JoiningRequestedEmployers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Company>().HasIndex(x => x.CompanyName).IsUnique();
        }
    }
}