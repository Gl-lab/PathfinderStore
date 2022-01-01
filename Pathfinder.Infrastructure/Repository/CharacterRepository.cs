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

        public async Task<ICollection<Character>> GetListAsync(int userId)
        {
            var account = await context.Set<Account>()
                .Where(e => e.UserId == userId)
                .FirstOrDefaultAsync();
            return account?.Characters;
        }

        public async Task<Character> GetCurrentAsync(int userId)
        {
            var account = await context.Set<Account>()
                .Where(e => e.UserId == userId)
                /*.Include(x => x.CurrentCharacter)
                .ThenInclude( x => x.Backpack)
                .ThenInclude( x=> x.Wallet)
                .Include(x => x.CurrentCharacter)
                .ThenInclude(x => x.Characteristics)*/
                .FirstOrDefaultAsync();
            return account?.CurrentCharacter;
        }
    }
}