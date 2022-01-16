using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.DTO;
using Pathfinder.Core.Entities.Authentication.User;

namespace Pathfinder.Application.Interfaces
{
    public interface IAccountService
    {
        Task UpdateAsync(AccountDto newAccount);
        Task CreateAsync(int userId);
        Task<AccountDto> GetCurrentAccountAsync();
        Task CreateCharacterAsync(CharacterDto newCharacter);
        Task DeleteCharacterAsync(int deletedCharacterId);
        Task<ICollection<CharacterDto>> GetCharactersByUserAsync(User user);
        Task<ICollection<CharacterDto>> GetCharactersByCurrentUserAsync();
        Task SetCurrentCharacterAsync(CharacterDto character);
        Task SetCurrentCharacterAsync(int characterId);
    }
}