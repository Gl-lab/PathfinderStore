using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Core.Entities.Auth.Roles;
using Pathfinder.Core.Entities.Auth.Users;
using Pathfinder.Core.Repositories.Auth;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Infrastructure.Paging;

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
