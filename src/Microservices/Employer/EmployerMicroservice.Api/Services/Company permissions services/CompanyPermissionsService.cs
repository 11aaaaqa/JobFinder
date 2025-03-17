using EmployerMicroservice.Api.Database;
using EmployerMicroservice.Api.Exceptions;
using EmployerMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployerMicroservice.Api.Services.Company_permissions_services
{
    public class CompanyPermissionsService(ApplicationDbContext context) : ICompanyPermissionsService
    {
        public async Task<List<string>> GetAllPermissionsAsync(Guid employerId, Guid companyId)
        {
            var companyPermission = await context.CompanyPermissions.Where(x => x.CompanyId == companyId)
                .SingleOrDefaultAsync(x => x.EmployerId == employerId);
            if (companyPermission is null)
                throw new CompanyPermissionDoesntExistException();
            return companyPermission.Permissions;
        }

        public async Task<bool> CheckForPermissionAsync(Guid employerId, Guid companyId, string permissionName)
        {
            var companyPermission = await context.CompanyPermissions.Where(x => x.CompanyId == companyId)
                .SingleOrDefaultAsync(x => x.EmployerId == employerId);
            if (companyPermission is null)
                throw new CompanyPermissionDoesntExistException();
            return companyPermission.Permissions.Contains(permissionName);
        }

        public async Task AddPermissions(Guid employerId, Guid companyId, List<string> permissions)
        {
            var companyPermission = await context.CompanyPermissions.Where(x => x.CompanyId == companyId)
                .SingleOrDefaultAsync(x => x.EmployerId == employerId);
            if (companyPermission is null)
            {
                await context.CompanyPermissions.AddAsync(new CompanyPermission
                    { CompanyId = companyId, EmployerId = employerId, Id = Guid.NewGuid(), Permissions = permissions });
            }
            else
            {
                companyPermission.Permissions = permissions;
            }
            await context.SaveChangesAsync();
        }
    }
}
