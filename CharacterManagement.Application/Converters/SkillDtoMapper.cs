using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class SkillDtoMapper
{
    public static SkillDto Map( SkillDefinition skill )
    {
        ArgumentNullException.ThrowIfNull( skill );

        return new SkillDto
        {
            Id = skill.Id,
            Name = skill.Name,
            KeyAbility = skill.KeyAbility,
        };
    }
}
