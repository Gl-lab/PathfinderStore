using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Services;

public interface ICharacterService
{
    Task<List<Character>> GetCharactersAsync(int userId);
        
    //   Task<ICollection<WeaponItemProperty>> WeaponItemProperty(int userId);
}