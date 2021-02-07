using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Auth.Roles;
using Pathfinder.Core.Repositories.Auth;
using Pathfinder.Infrastructure.Data;

namespace Pathfinder.Infrastructure.Repository.Auth
{
    public class RolePermissionsRepository: IRolePermissionsRepository
    {
        private readonly PgDbContext dbContext;
        public RolePermissionsRepository(PgDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddRangeAsync(IEnumerable<RolePermission> rolePermissions)
        {
            dbContext.AddRange(rolePermissions);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
