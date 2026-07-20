using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class LanguageRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsCompletePlayerCoreCatalog()
    {
        LanguageRepository repository = new LanguageRepository();

        IReadOnlyCollection<LanguageDefinition> languages = repository.GetAll();

        Assert.Equal( 33, languages.Count );
        Assert.Equal( languages.Count, languages.Select( language => language.Id ).Distinct().Count() );
        Assert.Equal( 11, languages.Count( language => language.Rarity == LanguageRarity.Common ) );
        Assert.Equal( 9, languages.Count( language => language.Category == LanguageCategory.Regional ) );
        Assert.All( languages, language => Assert.Equal( "Player Core", language.Source.Book ) );
    }

    [Theory]
    [InlineData( "common", LanguageRarity.Common, LanguageCategory.Standard )]
    [InlineData( "empyrean", LanguageRarity.Uncommon, LanguageCategory.Standard )]
    [InlineData( "varisian", LanguageRarity.Uncommon, LanguageCategory.Regional )]
    public void GetLanguage_ReturnsNormalizedDefinition(
        string languageId,
        LanguageRarity expectedRarity,
        LanguageCategory expectedCategory )
    {
        LanguageRepository repository = new LanguageRepository();

        LanguageDefinition language = repository.GetLanguage( languageId );

        Assert.Equal( languageId, language.Id.Value );
        Assert.Equal( expectedRarity, language.Rarity );
        Assert.Equal( expectedCategory, language.Category );
    }

    [Fact]
    public void GetLanguage_UnknownId_Throws()
    {
        LanguageRepository repository = new LanguageRepository();

        Assert.Throws<ArgumentOutOfRangeException>( () =>
            repository.GetLanguage( "unknown" ) );
    }
}
