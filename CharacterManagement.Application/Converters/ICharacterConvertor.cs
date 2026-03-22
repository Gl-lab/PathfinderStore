using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public interface ICharacterConvertor
{
    DraftCharacter Convert( CharacterDto character );
    CharacterDto Convert( DraftCharacter draftCharacter );
}