using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class ClericSpellDtoMapper
{
    public static ClericAvailableSpellDto MapAvailable( ClericAvailableSpell availableSpell )
    {
        ArgumentNullException.ThrowIfNull( availableSpell );

        return new ClericAvailableSpellDto
        {
            Spell = SpellDefinitionDtoMapper.Map( availableSpell.Spell ),
            EffectiveTradition = availableSpell.EffectiveTradition,
            AccessSources = availableSpell.AccessSources,
        };
    }
}
