using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Repositories;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Infrastructure.Repository.Base;

namespace Pathfinder.Infrastructure.Repository
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(PgDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<Account> GetByUserIdAsync(int UserId)
        {
            return await Table
                .Where(e => UserId == e.UserId)
                .Include(e => e.Characters)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }
    }
}
