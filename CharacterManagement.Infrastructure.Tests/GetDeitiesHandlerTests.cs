using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Deities;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetDeitiesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteCatalogIncludingNonClericFaith()
    {
        GetDeitiesHandler handler = new GetDeitiesHandler( new DeityRepository() );

        IReadOnlyCollection<DeityDto> result = await handler.Handle(
            new GetDeitiesCommand(),
            CancellationToken.None );

        Assert.Equal( 22, result.Count );
        Assert.False( Assert.Single( result, deity => deity.Id == "deity.atheism" ).CanGrantClericPowers );
        DeityDto iomedae = Assert.Single( result, deity => deity.Id == "deity.iomedae" );
        Assert.Equal( DivineSanctification.Holy, iomedae.RequiredSanctification );
        Assert.Equal( DivineFont.Heal, Assert.Single( iomedae.DivineFontOptions ) );
        Assert.Equal( "weapon.longsword", Assert.Single( iomedae.FavoredWeapons ).Id );
    }
}
