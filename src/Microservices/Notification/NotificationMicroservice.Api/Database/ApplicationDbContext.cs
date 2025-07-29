using Microsoft.EntityFrameworkCore;
using NotificationMicroservice.Api.Models;

namespace NotificationMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Notification> Notifications { get; set; }
    }
}
