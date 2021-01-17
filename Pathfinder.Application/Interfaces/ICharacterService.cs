using Pathfinder.Application.DTO;
using Pathfinder.Application.Models;
using Pathfinder.Core.Entities.Auth.Users;
using Pathfinder.Core.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pathfinder.Application.Interfaces
{
    public interface ICharacterService
    {
        Task<CharacterDto> GetCurrentCharacter(User user);
        Task<ICollection<CharacterDto>> GetCharactersByUserId(User user);
        Task SetCurrentCharacter(User user, int characterId);
        Task<CharacterDto> CreateCharacter(User user, CharacterDto newCharacter);
        Task DeleteCharacter(User user, CharacterDto character);
    }
}
