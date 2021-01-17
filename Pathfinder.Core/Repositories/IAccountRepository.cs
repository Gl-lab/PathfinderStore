using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Core.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetByUserIdAsync(int UserId);
    }
}
