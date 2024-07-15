using Authorization.Authentication.Role;

namespace Authorization.Repositories
{
    public interface IRolePermissionsRepository
    {
        Task AddRangeAsync(IEnumerable<RolePermission> rolePermissions);
    }
}
