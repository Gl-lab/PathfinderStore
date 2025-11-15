using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public interface ICharacterConvertor
{
    public DraftCharacter Convert( CharacterDto character );
    public CharacterDto Convert( DraftCharacter draftCharacter );
}