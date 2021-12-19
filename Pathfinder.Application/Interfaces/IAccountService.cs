using System.Collections.Generic;
using Pathfinder.Application.DTO;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Authentication.User;

namespace Pathfinder.Application.Interfaces
{
    public interface IAccountService
    {
        Task UpdateAsync(AccountDto newAccount);
        Task<AccountDto> GetCurrentAccountAsync();
        Task CreateCharacterAsync(CharacterDto newCharacter);
        Task DeleteCharacterAsync(int deletedCharacterId);
        Task<ICollection<CharacterDto>> GetCharactersByUserAsync(User user);
        Task<ICollection<CharacterDto>> GetCharactersByCurrentUserAsync();
        Task SetCurrentCharacterAsync(CharacterDto character);
        Task SetCurrentCharacterAsync(int characterId);
        
        
    }
}