using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.ArcaneTheses;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetArcaneThesesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteOrderedCatalogWithMilestones()
    {
        GetArcaneThesesHandler handler = new GetArcaneThesesHandler(
            new ArcaneThesisRepository() );

        IReadOnlyCollection<ArcaneThesisDto> result = await handler.Handle(
            new GetArcaneThesesCommand(),
            CancellationToken.None );

        Assert.Equal( 5, result.Count );
        Assert.Equal(
            result.Select( thesis => thesis.Name ).OrderBy( name => name ),
            result.Select( thesis => thesis.Name ) );
        ArcaneThesisDto familiar = Assert.Single(
            result.Where(
                thesis => thesis.Id == "arcane_thesis.improved_familiar_attunement" ) );
        ArcaneThesisEffectDto progression = Assert.Single(
            familiar.Effects.Where(
                effect => effect.Kind == ArcaneThesisEffectKind.FamiliarAbilityProgression ) );
        Assert.Equal( [ 1, 6, 12, 18 ], progression.MilestoneLevels );
    }
}
