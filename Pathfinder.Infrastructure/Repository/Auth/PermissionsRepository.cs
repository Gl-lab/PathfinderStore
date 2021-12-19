using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Core.Entities.Authentication.Permissions;
using Pathfinder.Core.Repositories.Auth;
using Pathfinder.Infrastructure.Data;

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
