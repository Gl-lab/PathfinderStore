using Pathfinder.Application.DTO;
using Pathfinder.Core.Entities.Auth.Users;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.DTO.Items;

namespace Pathfinder.Application.Interfaces
{
    public interface ICharacterService
    {
        Task<CharacterDto> GetCharacterAsync();
        Task<int> IncreaseBalance(int balance);
        Task<int> DecreaseBalance(int balance);
        Task EditCharacter(CharacterDto newCharacter);
        Task<ICollection<WeaponItemDto>> GetWeapons();

    }
}