using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class CharacterClassDtoMapper
{
    public static CharacterClassDto Map( CharacterClass characterClass )
    {
        ArgumentNullException.ThrowIfNull( characterClass );

        return new CharacterClassDto
        {
            Id = characterClass.Id,
            Name = characterClass.Name,
            Source = new SourceReferenceDto
            {
                Book = characterClass.Source.Book,
                Page = characterClass.Source.Page,
            },
            BaseHitPoints = characterClass.BaseHitPoints,
            KeyAbilityOptions = characterClass.KeyAbilityOptions.ToList(),
            SpellTradition = characterClass.SpellTradition,
            Rules = characterClass.Rules
                .Select( Map )
                .ToList(),
            DeferredDependencies = characterClass.DeferredDependencies.ToList(),
        };
    }

    public static CharacterClassPackageDto MapPackage(
        DraftCharacter character,
        CharacterClass characterClass )
    {
        ArgumentNullException.ThrowIfNull( character );
        ArgumentNullException.ThrowIfNull( characterClass );

        if ( !character.HasClassBoostPackage )
        {
            throw new InvalidOperationException( "Character does not have a complete class boost package." );
        }

        return new CharacterClassPackageDto
        {
            ClassId = characterClass.Id,
            Name = characterClass.Name,
            BaseHitPoints = characterClass.BaseHitPoints,
            KeyAbility = character.SelectedClassKeyAbility!.Value,
            Rules = characterClass.Rules
                .Select( Map )
                .ToList(),
        };
    }

    private static CharacterClassRuleDto Map( CharacterClassRuleDescriptor rule )
    {
        return new CharacterClassRuleDto
        {
            Id = rule.Id,
            Kind = rule.Kind,
            Name = rule.Name,
            Summary = rule.Summary,
            RequiresChoice = rule.RequiresChoice,
            DeferredDependencies = rule.DeferredDependencies.ToList(),
        };
    }
}
