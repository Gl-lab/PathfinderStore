using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Services.Implementation;

public sealed class CharacterService : ICharacterService
{
    private readonly ICharacterRepository _characterRepository;

    public CharacterService( ICharacterRepository characterRepository )
    {
        _characterRepository = characterRepository;
    }

    public async Task<List<DraftCharacter>> GetCharactersAsync( int userId )
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