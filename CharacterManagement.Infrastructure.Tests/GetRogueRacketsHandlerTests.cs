using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.RogueRackets;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetRogueRacketsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteOrderedCatalog()
    {
        GetRogueRacketsHandler handler = new GetRogueRacketsHandler( new RogueRacketRepository() );

        IReadOnlyCollection<RogueRacketDto> result = await handler.Handle(
            new GetRogueRacketsCommand(),
            CancellationToken.None );

        Assert.Equal( [ "Mastermind", "Ruffian", "Scoundrel", "Thief" ], result.Select( item => item.Name ) );
        RogueRacketDto ruffian = Assert.Single( result.Where( item => item.Id == "rogue_racket.ruffian" ) );
        Assert.Equal( AbilityType.Strength, ruffian.AlternativeKeyAbility );
        Assert.Equal( ProficiencyTargets.MediumArmor.Id, Assert.Single( ruffian.ProficiencyGrants ).TargetId );
    }
}
