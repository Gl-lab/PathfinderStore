using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Core.Entities.Auth.Permissions;
using Pathfinder.Core.Entities.Auth.Roles;
using Pathfinder.Core.Repositories.Auth;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Infrastructure.Paging;

namespace Pathfinder.Infrastructure.Repository.Auth
{
    public class PermissionsRepository: IPermissionsRepository
    {
        private readonly PgDbContext dbContext;
        public PermissionsRepository(PgDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ICollection<Permission>> GetListAsync()
        {
            return await dbContext
                .Permissions
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
