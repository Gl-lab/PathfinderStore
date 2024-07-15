using Authorization.Authentication.Role;
using Authorization.Repositories;
using Secure.Infrastructure.Data;

namespace Secure.Infrastructure.Repositories
{
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
}
