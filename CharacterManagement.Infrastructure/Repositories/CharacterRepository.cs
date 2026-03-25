using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Shared.Infrasturture.Repositories;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public class CharacterRepository : Repository<DraftCharacter>, ICharacterRepository
{
    public CharacterRepository( CharacterManagementDbContext dbContext )
        : base( dbContext )
    {
    }

    public async Task<List<DraftCharacter>> GetListAsync( int userId ) => throw new NotImplementedException( "CharacterRepository.GetListAsync" );

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