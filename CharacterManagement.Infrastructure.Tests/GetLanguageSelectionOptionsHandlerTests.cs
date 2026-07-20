using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Languages;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetLanguageSelectionOptionsHandlerTests
{
    [Fact]
    public async Task Handle_HumanWithIntelligenceFourteen_ReturnsThreeCommonChoicesWithoutCommon()
    {
        GetLanguageSelectionOptionsHandler handler = CreateHandler();

        LanguageSelectionOptionsDto result = await handler.Handle(
            new GetLanguageSelectionOptionsCommand( AncestryType.Human, 14 ),
            CancellationToken.None );

        Assert.Equal( 3, result.RequiredCount );
        Assert.DoesNotContain( result.AvailableLanguages, language => language.Id == "common" );
        Assert.Contains( result.AvailableLanguages, language => language.Id == "draconic" );
        Assert.All( result.AvailableLanguages, language => Assert.Equal( LanguageRarity.Common, language.Rarity ) );
    }

    [Fact]
    public async Task Handle_DwarfWithIntelligenceEight_ReturnsNoRequiredChoices()
    {
        GetLanguageSelectionOptionsHandler handler = CreateHandler();

        LanguageSelectionOptionsDto result = await handler.Handle(
            new GetLanguageSelectionOptionsCommand( AncestryType.Dwarf, 8 ),
            CancellationToken.None );

        Assert.Equal( 0, result.RequiredCount );
        Assert.DoesNotContain( result.AvailableLanguages, language => language.Id == "common" );
        Assert.DoesNotContain( result.AvailableLanguages, language => language.Id == "dwarven" );
    }

    private static GetLanguageSelectionOptionsHandler CreateHandler() =>
        new GetLanguageSelectionOptionsHandler(
            new AncestryRepository(),
            new LanguageRepository() );
}
