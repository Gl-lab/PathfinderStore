using Pathfinder.Secure.Application.Repositories;
using Pathfinder.Secure.Domain.Authentication.Role;
using Pathfinder.Secure.Infrastructure.Data;

namespace Pathfinder.Secure.Infrastructure.Repositories;

public class RolePermissionsRepository: IRolePermissionsRepository
{
    private readonly SecureDbContext _dbContext;
    public RolePermissionsRepository(SecureDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRangeAsync(IEnumerable<RolePermission> rolePermissions)
    {
        _dbContext.AddRange(rolePermissions);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}