using Microsoft.EntityFrameworkCore;
using VacancyMicroservice.Api.Models;

namespace VacancyMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Vacancy> Vacancies { get; set; }
    }
}
