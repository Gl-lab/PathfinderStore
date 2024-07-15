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
        public CharacterRepository(PathfinderDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<List<Character>> GetListAsync(int userId)
        {
            Account account = await context.Set<Account>()
                                           .Where(e => e.User.Id == userId)
                                           .FirstOrDefaultAsync();
            return account?.Characters;
        }

        // public async Task<Character> GetCurrentAsync(int userId)
        // {
        //     Account account = await context.Set<Account>()
        //                                    .Where(e => e.User.Id == userId)
        //                                     /*.Include(x => x.CurrentCharacter)
        //         .ThenInclude( x => x.Backpack)
        //         .ThenInclude( x=> x.Wallet)
        //         .Include(x => x.CurrentCharacter)
        //         .ThenInclude(x => x.Characteristics)*/
        //                                    .FirstOrDefaultAsync();
        //     return account?.CurrentCharacter;
        // }
    }
}