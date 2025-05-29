using BookmarkMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookmarkMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<FavoriteVacancy> FavoriteVacancies { get; set; }
    }
}
