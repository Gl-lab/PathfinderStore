using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Product;

namespace CharacterManagement.Application.Services
{
    public interface ICharacterService
    {
        Task<List<Character>> GetCharactersAsync(int userId);
        
     //   Task<ICollection<WeaponItemProperty>> WeaponItemProperty(int userId);
    }
}