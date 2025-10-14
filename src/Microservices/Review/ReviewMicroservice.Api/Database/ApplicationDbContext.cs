using Microsoft.EntityFrameworkCore;
using ReviewMicroservice.Api.Models;

namespace ReviewMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Review> Reviews { get; set; }
    }
}
