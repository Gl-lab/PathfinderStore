using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.ClericDoctrines;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetClericDoctrinesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteOrderedCatalog()
    {
        GetClericDoctrinesHandler handler = new GetClericDoctrinesHandler(
            new ClericDoctrineRepository() );

        IReadOnlyCollection<ClericDoctrineDto> result = await handler.Handle(
            new GetClericDoctrinesCommand(),
            CancellationToken.None );

        Assert.Equal( [ "Cloistered Cleric", "Warpriest" ], result.Select( item => item.Name ) );
        ClericDoctrineDto warpriest = Assert.Single(
            result.Where( item => item.Id == "cleric_doctrine.warpriest" ) );
        Assert.Equal( 3, warpriest.ProficiencyGrants.Count );
        Assert.Contains(
            warpriest.ProficiencyGrants,
            grant => ( grant.TargetId == ProficiencyTargets.Fortitude.Id ) &&
                     ( grant.Rank == ProficiencyRank.Expert ) );
        Assert.Contains(
            warpriest.Effects,
            effect => effect.Id == "cleric_doctrine.warpriest.effect.deadly_simplicity" );
    }
}
