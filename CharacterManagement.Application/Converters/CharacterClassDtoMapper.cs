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
            InitialProficiencies = MapProficiencies( characterClass.InitialProficiencies ),
            InitialSkillGrants = characterClass.InitialSkillGrants
                .Select( grant => new ClassSkillGrantDto
                {
                    Id = grant.Id,
                    SkillOptions = grant.SkillOptions.ToArray(),
                } )
                .ToList(),
            AdditionalSkillCountBase = characterClass.AdditionalSkillCountBase,
            SpellTradition = characterClass.SpellTradition,
            Rules = characterClass.Rules
                .Select( Map )
                .ToList(),
            DeferredDependencies = characterClass.DeferredDependencies.ToList(),
        };
    }

    public static IReadOnlyList<ProficiencyDto> MapProficiencies(
        IReadOnlyList<ProficiencyGrant> proficiencies )
    {
        ArgumentNullException.ThrowIfNull( proficiencies );

        return proficiencies
            .Select( grant => new ProficiencyDto
            {
                TargetId = grant.Target.Id,
                Name = grant.Target.Name,
                Category = grant.Target.Category,
                Rank = grant.Rank,
                SourceGrantId = grant.SourceGrantId,
                SourceGrantIds = [ grant.SourceGrantId ],
            } )
            .ToList();
    }

    public static IReadOnlyList<ProficiencyDto> MapProficiencies(
        IReadOnlyList<EffectiveProficiency> proficiencies )
    {
        ArgumentNullException.ThrowIfNull( proficiencies );

        return proficiencies
            .Select( proficiency => new ProficiencyDto
            {
                TargetId = proficiency.Target.Id,
                Name = proficiency.Target.Name,
                Category = proficiency.Target.Category,
                Rank = proficiency.Rank,
                SourceGrantId = proficiency.SourceGrantIds.First(),
                SourceGrantIds = proficiency.SourceGrantIds,
            } )
            .ToList();
    }

    public static CharacterClassPackageDto MapPackage(
        DraftCharacter character,
        CharacterClass characterClass,
        RogueRacket? rogueRacket = null,
        HuntersEdge? huntersEdge = null,
        DruidicOrder? druidicOrder = null,
        ClericDoctrine? clericDoctrine = null,
        Deity? deity = null )
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
            AdditionalSkillCount = characterClass.AdditionalSkillCountBase +
                                   character.AbilityScores.Intelligence.Modifier,
            RogueRacket = rogueRacket is null ? null : RogueRacketDtoMapper.MapPackage( rogueRacket ),
            HuntersEdge = huntersEdge is null ? null : HuntersEdgeDtoMapper.MapPackage( huntersEdge ),
            DruidicOrder = druidicOrder is null ? null : DruidicOrderDtoMapper.MapPackage( druidicOrder ),
            ClericDoctrine = clericDoctrine is null
                ? null
                : ClericDoctrineDtoMapper.MapPackage( clericDoctrine ),
            Deity = deity is null ? null : DeityDtoMapper.MapPackage( character, deity ),
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
