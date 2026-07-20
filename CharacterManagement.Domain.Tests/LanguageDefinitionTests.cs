using Pathfinder.CharacterManagement.Domain.Entity;

namespace CharacterManagement.Domain.Tests;

public sealed class LanguageDefinitionTests
{
    [Fact]
    public void Constructor_NormalizesDisplayValues()
    {
        LanguageDefinition language = new LanguageDefinition(
            new LanguageId( "common" ),
            " Common ",
            " Humans ",
            LanguageRarity.Common,
            LanguageCategory.Standard,
            new SourceReference( "Player Core", 89 ) );

        Assert.Equal( "common", language.Id.Value );
        Assert.Equal( "Common", language.Name );
        Assert.Equal( "Humans", language.Speakers );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( "Common" )]
    [InlineData( " common" )]
    [InlineData( "common language" )]
    [InlineData( "common-language" )]
    public void Constructor_InvalidStableId_Throws( string languageId )
    {
        Assert.Throws<ArgumentException>( () =>
            new LanguageDefinition(
                new LanguageId( languageId ),
                "Common",
                "Humans",
                LanguageRarity.Common,
                LanguageCategory.Standard,
                new SourceReference( "Player Core", 89 ) ) );
    }
}
