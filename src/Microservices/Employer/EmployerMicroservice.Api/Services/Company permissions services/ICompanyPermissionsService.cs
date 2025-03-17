namespace EmployerMicroservice.Api.Services.Company_permissions_services
{
    public interface ICompanyPermissionsService
    {
        Task<List<string>> GetAllPermissionsAsync(Guid employerId, Guid companyId);
        Task<bool> CheckForPermissionAsync(Guid employerId, Guid companyId, string permissionName);
        Task AddPermissions(Guid employerId, Guid companyId, List<string> permissions);
    }
}
