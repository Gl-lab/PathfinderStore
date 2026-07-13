using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class SkillRepository : ISkillRepository
{
    private static readonly Dictionary<string, SkillDefinition> Skills = CreateSkills()
        .ToDictionary( skill => skill.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<SkillDefinition> GetAll() => Skills.Values.ToList();

    public SkillDefinition GetSkill( string skillId )
    {
        if ( String.IsNullOrWhiteSpace( skillId ) )
        {
            throw new ArgumentException( "Skill id cannot be empty.", nameof( skillId ) );
        }

        if ( !Skills.TryGetValue( skillId, out SkillDefinition? skill ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( skillId ),
                $"Skill '{skillId}' is not defined." );
        }

        return skill;
    }

    private static IReadOnlyCollection<SkillDefinition> CreateSkills()
    {
        SourceReference source = new SourceReference( "Player Core", 227 );
        return
        [
            new SkillDefinition( "skill.acrobatics", "Acrobatics", AbilityType.Dexterity, source ),
            new SkillDefinition( "skill.arcana", "Arcana", AbilityType.Intelligence, source ),
            new SkillDefinition( "skill.athletics", "Athletics", AbilityType.Strength, source ),
            new SkillDefinition( "skill.crafting", "Crafting", AbilityType.Intelligence, source ),
            new SkillDefinition( "skill.deception", "Deception", AbilityType.Charisma, source ),
            new SkillDefinition( "skill.diplomacy", "Diplomacy", AbilityType.Charisma, source ),
            new SkillDefinition( "skill.intimidation", "Intimidation", AbilityType.Charisma, source ),
            new SkillDefinition( "skill.medicine", "Medicine", AbilityType.Wisdom, source ),
            new SkillDefinition( "skill.nature", "Nature", AbilityType.Wisdom, source ),
            new SkillDefinition( "skill.occultism", "Occultism", AbilityType.Intelligence, source ),
            new SkillDefinition( "skill.performance", "Performance", AbilityType.Charisma, source ),
            new SkillDefinition( "skill.religion", "Religion", AbilityType.Wisdom, source ),
            new SkillDefinition( "skill.society", "Society", AbilityType.Intelligence, source ),
            new SkillDefinition( "skill.stealth", "Stealth", AbilityType.Dexterity, source ),
            new SkillDefinition( "skill.survival", "Survival", AbilityType.Wisdom, source ),
            new SkillDefinition( "skill.thievery", "Thievery", AbilityType.Dexterity, source ),
        ];
    }
}
