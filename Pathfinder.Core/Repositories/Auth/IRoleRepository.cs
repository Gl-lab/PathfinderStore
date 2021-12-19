using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Authentication.Role;

namespace Pathfinder.Core.Repositories.Auth
{
    public interface IRoleRepository
    {
         Task<ICollection<Role>> GetListAsync();
    }
}
