using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.ClericSpells;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetClericSpellOptionsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsServerResolvedOptionsForSelectedDeity()
    {
        GetClericSpellOptionsHandler handler = new GetClericSpellOptionsHandler(
            new DeityRepository(),
            new SpellRepository() );

        ClericSpellOptionsDto result = await handler.Handle(
            new GetClericSpellOptionsCommand( "deity.iomedae" ),
            CancellationToken.None );

        Assert.Equal( 16, result.Cantrips.Count );
        Assert.Equal( 24, result.RankOneSpells.Count );
        ClericAvailableSpellDto sureStrike = Assert.Single(
            result.RankOneSpells,
            spell => spell.Spell.Id == "spell.sure_strike" );
        Assert.Equal( SpellTradition.Divine, sureStrike.EffectiveTradition );
        Assert.Equal( [ ClericSpellAccessSource.DeityGranted ], sureStrike.AccessSources );
        Assert.DoesNotContain( result.RankOneSpells, spell => spell.Spell.Kind == SpellKind.Focus );
    }
}
