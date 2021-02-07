using Pathfinder.Application.DTO;
using Pathfinder.Core.Entities.Auth.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Application.Interfaces
{
    public interface ICharacterService
    {
         Task<CharacterDto> GetCurrentCharacterAsync();
        Task<ICollection<CharacterDto>> GetCharactersByUserAsync(User user);
        Task<ICollection<CharacterDto>> GetCharactersByCurrentUserAsync();
        Task SetCurrentCharacterAsync(int characterId);
        Task SetCurrentCharacterAsync(CharacterDto character);
        Task CreateCharacterAsync(CharacterDto newCharacter);
        Task DeleteCharacterAsync(int deletedCharacterId);
    }
}
