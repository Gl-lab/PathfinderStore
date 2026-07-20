using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.CharacterManagement.Domain.Rules.Languages;

namespace CharacterManagement.Domain.Tests;

public sealed class LanguageSelectionResolverTests
{
    [Theory]
    [InlineData( -1, 1 )]
    [InlineData( 0, 1 )]
    [InlineData( 2, 3 )]
    public void ResolveOptions_HumanUsesOnePlusPositiveIntelligenceModifier(
        int intelligenceModifier,
        int expectedCount )
    {
        Ancestry human = CreateAncestry(
            AncestryType.Human,
            [ LanguageIds.Common ],
            new AdditionalLanguageRule(
                AdditionalLanguageRuleType.OnePlusIntelligenceModifier,
                [],
                true,
                true ) );

        LanguageSelectionOptions options = LanguageSelectionResolver.ResolveOptions(
            human,
            intelligenceModifier,
            Catalog() );

        Assert.Equal( expectedCount, options.RequiredCount );
        Assert.DoesNotContain( LanguageIds.Common, options.AvailableLanguageIds );
        Assert.All(
            options.AvailableLanguageIds,
            languageId => Assert.Contains(
                languageId,
                new[] { LanguageIds.Draconic, LanguageIds.Dwarven, LanguageIds.Elven } ) );
    }

    [Fact]
    public void ResolveOptions_AccessLanguageIsAvailableOnlyFromTrustedContext()
    {
        Ancestry dwarf = CreateAncestry(
            AncestryType.Dwarf,
            [ LanguageIds.Common, LanguageIds.Dwarven ],
            new AdditionalLanguageRule(
                AdditionalLanguageRuleType.IntelligenceModifier,
                [ LanguageIds.Elven ],
                false,
                true ) );

        LanguageSelectionOptions withoutAccess = LanguageSelectionResolver.ResolveOptions(
            dwarf,
            1,
            Catalog() );
        LanguageSelectionOptions withAccess = LanguageSelectionResolver.ResolveOptions(
            dwarf,
            1,
            Catalog(),
            [ LanguageIds.Aklo ] );

        Assert.DoesNotContain( LanguageIds.Aklo, withoutAccess.AvailableLanguageIds );
        Assert.Contains( LanguageIds.Aklo, withAccess.AvailableLanguageIds );
    }

    [Fact]
    public void ResolveSelection_DuplicateLanguages_Throws()
    {
        Ancestry human = CreateAncestry(
            AncestryType.Human,
            [ LanguageIds.Common ],
            new AdditionalLanguageRule(
                AdditionalLanguageRuleType.OnePlusIntelligenceModifier,
                [],
                true,
                true ) );

        Assert.Throws<CharacterManagementException>( () =>
            LanguageSelectionResolver.ResolveSelection(
                human,
                1,
                Catalog(),
                [ "draconic", "draconic" ] ) );
    }

    [Fact]
    public void ResolveSelection_StartingOrUnavailableLanguage_Throws()
    {
        Ancestry dwarf = CreateAncestry(
            AncestryType.Dwarf,
            [ LanguageIds.Common, LanguageIds.Dwarven ],
            new AdditionalLanguageRule(
                AdditionalLanguageRuleType.IntelligenceModifier,
                [ LanguageIds.Elven ],
                false,
                true ) );

        Assert.Throws<CharacterManagementException>( () =>
            LanguageSelectionResolver.ResolveSelection(
                dwarf,
                1,
                Catalog(),
                [ "dwarven" ] ) );
        Assert.Throws<CharacterManagementException>( () =>
            LanguageSelectionResolver.ResolveSelection(
                dwarf,
                1,
                Catalog(),
                [ "aklo" ] ) );
    }

    private static Ancestry CreateAncestry(
        AncestryType ancestryType,
        IReadOnlyList<LanguageId> startingLanguages,
        AdditionalLanguageRule rule ) => new Ancestry(
            ancestryType,
            [],
            [],
            8,
            RaceSizeType.Medium,
            25,
            startingLanguages: startingLanguages,
            additionalLanguageRule: rule );

    private static IReadOnlyCollection<LanguageDefinition> Catalog()
    {
        SourceReference source = new SourceReference( "Player Core", 89 );
        return
        [
            Language( LanguageIds.Common, "Common", LanguageRarity.Common, source ),
            Language( LanguageIds.Draconic, "Draconic", LanguageRarity.Common, source ),
            Language( LanguageIds.Dwarven, "Dwarven", LanguageRarity.Common, source ),
            Language( LanguageIds.Elven, "Elven", LanguageRarity.Common, source ),
            Language( LanguageIds.Aklo, "Aklo", LanguageRarity.Uncommon, source ),
        ];
    }

    private static LanguageDefinition Language(
        LanguageId id,
        string name,
        LanguageRarity rarity,
        SourceReference source ) => new LanguageDefinition(
            id,
            name,
            name,
            rarity,
            LanguageCategory.Standard,
            source );
}
