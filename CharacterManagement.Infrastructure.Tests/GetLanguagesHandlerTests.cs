using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Languages;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetLanguagesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteCatalogSortedByName()
    {
        GetLanguagesHandler handler = new GetLanguagesHandler( new LanguageRepository() );

        IReadOnlyCollection<LanguageDto> result = await handler.Handle(
            new GetLanguagesCommand(),
            CancellationToken.None );

        Assert.Equal( 33, result.Count );
        Assert.Equal(
            result.Select( language => language.Name ).OrderBy( name => name ),
            result.Select( language => language.Name ) );
        LanguageDto common = Assert.Single( result, language => language.Id == "common" );
        Assert.Equal( LanguageRarity.Common, common.Rarity );
        Assert.Equal( 89, common.Source.Page );
    }
}
