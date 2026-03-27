using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.Utils.Repositories.Base;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface ICharacterRepository : IRepository<DraftCharacter>
{
    Task<List<DraftCharacter>> GetListAsync( int userId );
    Task<DraftCharacter?> GetByIdAsync( int id, int userId );

    // Task<Character> GetCurrentAsync(int UserId);
}
