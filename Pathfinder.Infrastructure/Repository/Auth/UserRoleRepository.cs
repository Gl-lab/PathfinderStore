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
    public class UserRoleRepository: IUserRoleRepository
    {
        private readonly PgDbContext dbContext;
        public UserRoleRepository(PgDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddRangeAsync(IEnumerable<UserRole> userRoles)
        {
            dbContext.AddRange(userRoles);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
