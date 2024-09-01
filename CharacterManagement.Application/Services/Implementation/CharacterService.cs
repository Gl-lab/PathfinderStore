using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Services.Implementation;

public sealed class CharacterService : ICharacterService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly IWeaponItemPropertyRepository _weaponItemPropertyRepository;

    public CharacterService( ICharacterRepository characterRepository,
                             IWeaponItemPropertyRepository weaponItemPropertyRepository )
    {
        _characterRepository = characterRepository;
        _weaponItemPropertyRepository = weaponItemPropertyRepository;
    }

    public async Task<List<Character>> GetCharactersAsync( int userId )
    {
        return await _characterRepository.GetListAsync( userId );
    }

      

    public async Task EditCharacter( CharacterDto newCharacter )
    {
        // var character = await _characterRepository.GetCurrentAsync(_userService.GetCurrentUser().Id);
        // if (newCharacter.Name != character.Name ) character.Rename(newCharacter.Name);
        // if (newCharacter.RaceId != character.RaceId) character.ChangeRace(newCharacter.RaceId);
    }

    // public async Task<ICollection<WeaponItemProperty>> WeaponItemProperty( int userId )
    // {
    //     Character character = await _characterRepository.GetCurrentAsync( userId );
    //     if ( character == null )
    //     {
    //         throw new ApplicationException( "User dont have current character" );
    //     }
    //
    //     return await _weaponItemPropertyRepository.GetWeaponItemByItemIdCollection(
    //         character
    //            .Inventory
    //            .Items
    //            .Where( e => e.Item.Product.CategoryType == CategoryType.Weapon )
    //            .Select( e => e.Item.Id )
    //            .ToList() );
    // }
}