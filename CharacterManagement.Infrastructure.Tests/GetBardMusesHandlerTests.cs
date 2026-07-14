using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.BardMuses;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetBardMusesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteOrderedCatalog()
    {
        GetBardMusesHandler handler = new GetBardMusesHandler(
            new BardMuseRepository() );

        IReadOnlyCollection<BardMuseDto> result = await handler.Handle(
            new GetBardMusesCommand(),
            CancellationToken.None );

        Assert.Equal( [ "Enigma", "Maestro", "Polymath", "Warrior" ], result.Select( item => item.Name ) );
        BardMuseDto enigma = Assert.Single(
            result.Where( item => item.Id == "bard_muse.enigma" ) );
        Assert.Contains( enigma.Benefits, benefit => benefit.Id == "feat.bardic_lore" );
        Assert.Contains( enigma.Benefits, benefit => benefit.Id == "spell.sure_strike" );
    }
}
