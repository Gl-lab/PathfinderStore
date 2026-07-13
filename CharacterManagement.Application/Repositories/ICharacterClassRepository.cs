using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface ICharacterClassRepository
{
    IReadOnlyCollection<CharacterClass> GetAll();
    CharacterClass GetCharacterClass( string characterClassId );
}
