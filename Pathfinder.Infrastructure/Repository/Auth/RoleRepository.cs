using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Core.Entities.Authentication.Role;
using Pathfinder.Core.Repositories.Auth;
using Pathfinder.Infrastructure.Data;

namespace Pathfinder.Infrastructure.Repository.Auth
{
    public class RoleRepository: IRoleRepository
    {
        private readonly PgDbContext dbContext;
        public RoleRepository(PgDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ICollection<Role>> GetListAsync()
        {
            return await dbContext
                .Roles
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
