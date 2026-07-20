using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Converters;

public static class AncestryDtoMapper
{
    public static AncestryDto Map(
        Ancestry ancestry,
        IAncestryChoiceAvailabilityPolicy availabilityPolicy )
    {
        ArgumentNullException.ThrowIfNull( ancestry );
        ArgumentNullException.ThrowIfNull( availabilityPolicy );

        return new AncestryDto
        {
            Type = ancestry.AncestryType,
            Name = ancestry.AncestryType.ToString(),
            Source = Map( ancestry.Source ),
            AbilityBoosts = ancestry.AbilityBoosts
                .Select( Map )
                .ToList(),
            AbilityFlaws = ancestry.AbilityFlaws.ToList(),
            BaseHitPoints = ancestry.BaseHitPoints,
            Size = ancestry.Size,
            BaseSpeed = ancestry.BaseSpeed,
            Vision = ancestry.Vision,
            Darkvision = ancestry.Darkvision,
            LowLightVision = ancestry.LowLightVision,
            StartingLanguageIds = ancestry.StartingLanguages
                .Select( languageId => languageId.Value )
                .ToList(),
            AdditionalLanguageRule = ancestry.AdditionalLanguageRule is null
                ? null
                : Map( ancestry.AdditionalLanguageRule ),
            GrantedItems = ancestry.GrantedItems
                .Select( Map )
                .ToList(),
            GrantedRules = ancestry.GrantedRules
                .Select( Map )
                .ToList(),
            Heritages = ancestry.Heritages
                .Where( availabilityPolicy.IsAvailable )
                .Select( Map )
                .ToList(),
            AncestryFeats = ancestry.AncestryFeats
                .Where( ancestryFeat => ancestryFeat.Level == 1 )
                .Where( availabilityPolicy.IsAvailable )
                .Select( Map )
                .ToList(),
        };
    }

    public static CharacterAncestryPackageDto MapPackage( DraftCharacter character, Ancestry ancestry )
    {
        ArgumentNullException.ThrowIfNull( character );
        ArgumentNullException.ThrowIfNull( ancestry );

        Heritage? heritage = ancestry.Heritages
            .SingleOrDefault( item => item.Id == character.SelectedHeritageId );
        AncestryFeat? ancestryFeat = ancestry.AncestryFeats
            .SingleOrDefault( item => item.Id == character.SelectedAncestryFeatId );
        List<AncestryEffectDescriptor> selectedEffects = [];

        if ( heritage is not null )
        {
            selectedEffects.AddRange( heritage.Effects );
        }

        if ( ancestryFeat is not null )
        {
            selectedEffects.AddRange( ancestryFeat.Effects );
        }

        VisionType? visionOverride = selectedEffects
            .Select( effect => effect.VisionOverride )
            .FirstOrDefault( vision => vision is not null );
        return new CharacterAncestryPackageDto
        {
            SelectedHeritageId = character.SelectedHeritageId,
            SelectedAncestryFeatId = character.SelectedAncestryFeatId,
            EffectiveVision = visionOverride ?? ancestry.Vision,
            EffectiveBaseHitPoints = ancestry.GetEffectiveBaseHitPoints(
                character.SelectedHeritageId,
                character.SelectedAncestryFeatId ),
            StartingLanguageIds = ancestry.StartingLanguages
                .Select( languageId => languageId.Value )
                .ToList(),
            AdditionalLanguageIds = character.SelectedAdditionalLanguageIds.ToArray(),
            KnownLanguageIds = ancestry.StartingLanguages
                .Select( languageId => languageId.Value )
                .Concat( character.SelectedAdditionalLanguageIds )
                .Distinct( StringComparer.Ordinal )
                .ToArray(),
            AdditionalLanguageRule = ancestry.AdditionalLanguageRule is null
                ? null
                : Map( ancestry.AdditionalLanguageRule ),
            GrantedItems = ancestry.GrantedItems
                .Select( Map )
                .ToList(),
            GrantedRules = ancestry.GrantedRules
                .Select( Map )
                .ToList(),
            SelectedEffects = selectedEffects
                .Select( Map )
                .ToList(),
            DeferredDependencies = selectedEffects
                .SelectMany( effect => GetDeferredDependencies( heritage, ancestryFeat, effect ) )
                .Distinct()
                .ToList(),
        };
    }

    private static IEnumerable<AncestryDependencyType> GetDeferredDependencies(
        Heritage? heritage,
        AncestryFeat? ancestryFeat,
        AncestryEffectDescriptor effect )
    {
        if ( heritage is not null && heritage.Effects.Contains( effect ) )
        {
            return heritage.DeferredDependencies;
        }

        if ( ancestryFeat is not null && ancestryFeat.Effects.Contains( effect ) )
        {
            return ancestryFeat.DeferredDependencies;
        }

        return [];
    }

    private static SourceReferenceDto Map( SourceReference source ) => new SourceReferenceDto
    {
        Book = source.Book,
        Page = source.Page,
    };

    private static AdditionalLanguageRuleDto Map( AdditionalLanguageRule rule ) => new AdditionalLanguageRuleDto
    {
        Type = rule.Type,
        AllowedLanguageIds = rule.AllowedLanguageIds
            .Select( languageId => languageId.Value )
            .ToList(),
        AllowsCommonLanguages = rule.AllowsCommonLanguages,
        AllowsAccessLanguages = rule.AllowsAccessLanguages,
    };

    private static GrantedItemDto Map( GrantedItem item ) => new GrantedItemDto
    {
        ItemId = item.ItemId,
        Quantity = item.Quantity,
        Source = Map( item.Source ),
    };

    private static GrantedRuleDto Map( GrantedRule rule ) => new GrantedRuleDto
    {
        RuleId = rule.RuleId,
        EffectKind = rule.EffectKind,
        Summary = rule.Summary,
    };

    private static HeritageDto Map( Heritage heritage ) => new HeritageDto
    {
        Id = heritage.Id,
        Name = heritage.Name,
        Source = Map( heritage.Source ),
        Rarity = heritage.Rarity,
        Restrictions = heritage.Restrictions.ToList(),
        IncompatibleChoiceIds = heritage.IncompatibleChoiceIds.ToList(),
        Effects = heritage.Effects
            .Select( Map )
            .ToList(),
        DeferredDependencies = heritage.DeferredDependencies.ToList(),
    };

    private static AncestryFeatDto Map( AncestryFeat ancestryFeat ) => new AncestryFeatDto
    {
        Id = ancestryFeat.Id,
        Name = ancestryFeat.Name,
        Source = Map( ancestryFeat.Source ),
        Level = ancestryFeat.Level,
        Rarity = ancestryFeat.Rarity,
        Prerequisites = ancestryFeat.Prerequisites.ToList(),
        Restrictions = ancestryFeat.Restrictions.ToList(),
        IncompatibleChoiceIds = ancestryFeat.IncompatibleChoiceIds.ToList(),
        Effects = ancestryFeat.Effects
            .Select( Map )
            .ToList(),
        DeferredDependencies = ancestryFeat.DeferredDependencies.ToList(),
    };

    private static AncestryEffectDto Map( AncestryEffectDescriptor effect ) => new AncestryEffectDto
    {
        EffectId = effect.EffectId,
        EffectKind = effect.EffectKind,
        Summary = effect.Summary,
        VisionOverride = effect.VisionOverride,
        BaseHitPointsOverride = effect.BaseHitPointsOverride,
    };

    private static AncestryBoostDto Map( AncestryBoostSlot slot ) => new AncestryBoostDto
    {
        AbilityType = slot is AncestryBoostSlot.FixedBoost fixedBoost ? fixedBoost.AbilityType : null,
        IsFree = slot is AncestryBoostSlot.FreeBoost,
    };
}
