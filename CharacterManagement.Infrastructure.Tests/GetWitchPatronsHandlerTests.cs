using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.WitchPatrons;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetWitchPatronsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteOrderedCatalogWithNestedChoice()
    {
        GetWitchPatronsHandler handler = new GetWitchPatronsHandler(
            new WitchPatronRepository() );

        IReadOnlyCollection<WitchPatronDto> result = await handler.Handle(
            new GetWitchPatronsCommand(),
            CancellationToken.None );

        Assert.Equal( 7, result.Count );
        WitchPatronDto wilding = Assert.Single(
            result.Where( item => item.Id == "witch_patron.wilding_steward" ) );
        Assert.Equal( SpellTradition.Primal, wilding.SpellTradition );
        Assert.Equal( "skill.nature", Assert.Single( wilding.SkillGrant.SkillOptions ) );
        Assert.Equal(
            [ "spell.summon_animal", "spell.summon_plant_or_fungus" ],
            wilding.Benefits
                .Where( benefit => benefit.Kind == WitchPatronBenefitKind.FamiliarSpell )
                .Select( benefit => benefit.Id ) );
    }
}
