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
        public AccountRepository( PathfinderDbContext dbContext )
            : base( dbContext )
        {
        }

        public async Task<Account> GetByUserIdAsync( int userId )
        {
            return await Table
                        .Where( e => userId == e.User.Id )
                        .Include( e => e.Characters )
                        .FirstOrDefaultAsync();
        }

        public async Task<Account> GetByCharacterIdAsync( int characterId )
        {
            return await Table
                 .Where( e => e.Characters.Any( x => x.Id == characterId ) )
                 .Include( e => e.Characters )
                 .FirstOrDefaultAsync();
        }
    }
}