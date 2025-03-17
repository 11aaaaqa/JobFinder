namespace EmployerMicroservice.Api.Services.Company_permissions_services
{
    public interface ICompanyPermissionsService
    {
        Task<List<string>> GetAllPermissionsAsync(Guid employerId);
        Task<bool> CheckForPermissionAsync(Guid employerId, string permissionName);
        Task AddPermissions(Guid employerId, List<string> permissions);
    }
}
