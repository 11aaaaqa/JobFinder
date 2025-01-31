using EmployeeMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
    }
}