using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface ISpellRepository
{
    IReadOnlyCollection<SpellDefinition> GetAll();
    SpellDefinition GetSpell( string spellId );
}
