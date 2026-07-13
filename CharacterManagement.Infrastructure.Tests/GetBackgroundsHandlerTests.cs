using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Backgrounds;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetBackgroundsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteCatalogSortedByName()
    {
        GetBackgroundsHandler handler = new GetBackgroundsHandler( new BackgroundRepository() );

        IReadOnlyCollection<BackgroundDto> result = await handler.Handle(
            new GetBackgroundsCommand(),
            CancellationToken.None );

        Assert.Equal( 35, result.Count );
        Assert.Equal(
            result.Select( background => background.Name ).OrderBy( name => name ),
            result.Select( background => background.Name ) );
    }

    [Fact]
    public async Task Handle_MapsBoostsAndDeclarativeGrants()
    {
        GetBackgroundsHandler handler = new GetBackgroundsHandler( new BackgroundRepository() );

        IReadOnlyCollection<BackgroundDto> result = await handler.Handle(
            new GetBackgroundsCommand(),
            CancellationToken.None );
        BackgroundDto acrobat = Assert.Single(
            result.Where( background => background.Id == "background.acrobat" ) );

        Assert.Equal( [ AbilityType.Strength, AbilityType.Dexterity ], acrobat.RestrictedBoostOptions );
        Assert.Equal( 1, acrobat.FreeBoostCount );
        Assert.Contains( acrobat.Grants, grant => grant.TargetId == "skill.acrobatics" );
        Assert.Contains( acrobat.Grants, grant => grant.TargetId == "lore.circus" );
        Assert.Contains( acrobat.Grants, grant => grant.TargetId == "skill_feat.steady_balance" );
    }
}
