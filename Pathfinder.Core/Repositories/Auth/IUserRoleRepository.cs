using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Auth.Roles;
using Pathfinder.Core.Entities.Auth.Users;
using Pathfinder.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Core.Repositories.Auth
{
    public interface IUserRoleRepository
    {
        Task AddRangeAsync(IEnumerable<UserRole> userRoles);
    }
}
