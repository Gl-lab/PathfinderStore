using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class AncestryLanguageRuleTests
{
    [Fact]
    public void AllAncestryLanguageReferences_ExistInLanguageCatalog()
    {
        AncestryRepository ancestryRepository = new AncestryRepository();
        LanguageRepository languageRepository = new LanguageRepository();
        HashSet<string> languageIds = languageRepository
            .GetAll()
            .Select( language => language.Id.Value )
            .ToHashSet( StringComparer.Ordinal );

        foreach ( Ancestry ancestry in ancestryRepository.GetAll() )
        {
            Assert.All(
                ancestry.StartingLanguages,
                languageId => Assert.Contains( languageId.Value, languageIds ) );
            Assert.NotNull( ancestry.AdditionalLanguageRule );
            Assert.All(
                ancestry.AdditionalLanguageRule.AllowedLanguageIds,
                languageId => Assert.Contains( languageId.Value, languageIds ) );
            Assert.True( ancestry.AdditionalLanguageRule.AllowsAccessLanguages );
        }
    }

    [Fact]
    public void HumanRule_AllowsCommonAndAccessLanguagesWithoutGrantingAllUncommonLanguages()
    {
        Ancestry human = new AncestryRepository().GetAncestry( AncestryType.Human );

        Assert.Equal( [ LanguageIds.Common ], human.StartingLanguages );
        Assert.NotNull( human.AdditionalLanguageRule );
        Assert.Equal(
            AdditionalLanguageRuleType.OnePlusIntelligenceModifier,
            human.AdditionalLanguageRule.Type );
        Assert.Empty( human.AdditionalLanguageRule.AllowedLanguageIds );
        Assert.True( human.AdditionalLanguageRule.AllowsCommonLanguages );
        Assert.True( human.AdditionalLanguageRule.AllowsAccessLanguages );
    }

    [Fact]
    public void DwarfRule_UsesFixedPoolAndAccessLanguages()
    {
        Ancestry dwarf = new AncestryRepository().GetAncestry( AncestryType.Dwarf );

        Assert.Equal( [ LanguageIds.Common, LanguageIds.Dwarven ], dwarf.StartingLanguages );
        Assert.NotNull( dwarf.AdditionalLanguageRule );
        Assert.False( dwarf.AdditionalLanguageRule.AllowsCommonLanguages );
        Assert.True( dwarf.AdditionalLanguageRule.AllowsAccessLanguages );
        Assert.Contains( LanguageIds.Petran, dwarf.AdditionalLanguageRule.AllowedLanguageIds );
    }
}
