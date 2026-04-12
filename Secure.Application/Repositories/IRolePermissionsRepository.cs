using Pathfinder.Secure.Domain.Authentication.Role;

namespace Pathfinder.Secure.Application.Repositories;

public interface IRolePermissionsRepository
{
    Task AddRangeAsync(IEnumerable<RolePermission> rolePermissions);
}