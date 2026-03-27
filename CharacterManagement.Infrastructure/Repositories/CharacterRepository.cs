using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Shared.Infrasturture.Repositories;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public class CharacterRepository : Repository<DraftCharacter>, ICharacterRepository
{
    private readonly CharacterManagementDbContext _dbContext;

    public CharacterRepository( CharacterManagementDbContext dbContext )
        : base( dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<List<DraftCharacter>> GetListAsync( int userId )
        => await _dbContext.Character
            .Where( c => c.Account.UserId == userId )
            .ToListAsync();

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