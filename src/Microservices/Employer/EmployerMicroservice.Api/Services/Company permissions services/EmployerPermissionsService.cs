using EmployerMicroservice.Api.Database;
using EmployerMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployerMicroservice.Api.Services.Company_permissions_services
{
    public class EmployerPermissionsService(ApplicationDbContext context) : IEmployerPermissionsService
    {
        public async Task<List<string>> GetAllPermissionsAsync(Guid employerId)
        {
            var employerPermissions = await context.EmployersPermissions
                .SingleOrDefaultAsync(x => x.EmployerId == employerId);
            if (employerPermissions is null)
                return new List<string>();
            return employerPermissions.Permissions;
        }

        public async Task<bool> CheckForPermissionAsync(Guid employerId, string permissionName)
        {
            var employerPermissions = await context.EmployersPermissions
                .SingleOrDefaultAsync(x => x.EmployerId == employerId);
            if (employerPermissions is null)
                return false;
            return employerPermissions.Permissions.Contains(permissionName);
        }

        public async Task AddPermissions(Guid employerId, List<string> permissions)
        {
            var employerPermissions = await context.EmployersPermissions
                .SingleOrDefaultAsync(x => x.EmployerId == employerId);
            if (employerPermissions is null)
            {
                await context.EmployersPermissions.AddAsync(new EmployerPermissions
                    { EmployerId = employerId, Permissions = permissions });
            }
            else
            {
                employerPermissions.Permissions = permissions;
            }
            await context.SaveChangesAsync();
        }

        public async Task RemoveAllEmployerPermissions(Guid employerId)
        {
            var employerPermissions = await context.EmployersPermissions.SingleOrDefaultAsync(x => x.EmployerId == employerId);
            if (employerPermissions is null)
                return;
            context.EmployersPermissions.Remove(employerPermissions);
            await context.SaveChangesAsync();
        }
    }
}
