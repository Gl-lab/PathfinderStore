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
    public class CharacterRepository : Repository<Character>, ICharacterRepository
    {
        public CharacterRepository(PgDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<ICollection<Character>> GetListAsync(int UserId)
        {
            var account = await context.Set<Account>()
                .Where(e => e.UserId == UserId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            return account?.Characters;
        }

        public async Task<Character> GetCurrentAsync(int UserId)
        {
            var account = await context.Set<Account>()
                .Where(e => e.UserId == UserId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            return account?.CurrentCharacter;
        }
    }
}
