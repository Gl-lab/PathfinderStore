using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Skills;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetSkillsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteCatalogSortedByName()
    {
        GetSkillsHandler handler = new GetSkillsHandler( new SkillRepository() );

        IReadOnlyCollection<SkillDto> result = await handler.Handle(
            new GetSkillsCommand(),
            CancellationToken.None );

        Assert.Equal( 16, result.Count );
        Assert.Equal(
            result.Select( skill => skill.Name ).OrderBy( name => name ),
            result.Select( skill => skill.Name ) );
        SkillDto nature = Assert.Single( result, skill => skill.Id == "skill.nature" );
        Assert.Equal( AbilityType.Wisdom, nature.KeyAbility );
    }
}
