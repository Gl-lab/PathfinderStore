using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Core.Entities.Auth.Roles;
using Pathfinder.Core.Repositories.Auth;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Infrastructure.Paging;

namespace Pathfinder.Infrastructure.Repository.Auth
{
    public class RoleRepository: IRoleRepository
    {
        private readonly PgDbContext dbContext;
        public RoleRepository(PgDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ICollection<Role>> GetRolesListAsync()
        {
            return await dbContext
                .Roles
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
