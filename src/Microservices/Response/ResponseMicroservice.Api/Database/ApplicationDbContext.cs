using Microsoft.EntityFrameworkCore;
using ResponseMicroservice.Api.Models;

namespace ResponseMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<InterviewInvitation> InterviewInvitations { get; set; }
        public DbSet<VacancyResponse> VacancyResponses { get; set; }
    }
}
