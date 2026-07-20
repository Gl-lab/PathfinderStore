using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.Spells;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetSpellOptionsHandlerTests
{
    [Theory]
    [InlineData( SpellTradition.Arcane, SpellKind.Cantrip, 19 )]
    [InlineData( SpellTradition.Occult, SpellKind.Spell, 33 )]
    [InlineData( SpellTradition.Primal, SpellKind.Spell, 31 )]
    public async Task Handle_ReturnsCommonOptionsForRequestedTradition(
        SpellTradition tradition,
        SpellKind kind,
        int expectedCount )
    {
        GetSpellOptionsHandler handler = new GetSpellOptionsHandler( new SpellRepository() );

        IReadOnlyCollection<SpellDefinitionDto> result = await handler.Handle(
            new GetSpellOptionsCommand( tradition, 1, kind ),
            CancellationToken.None );

        Assert.Equal( expectedCount, result.Count );
        Assert.All( result, spell => Assert.Contains( tradition, spell.Traditions ) );
        Assert.All( result, spell => Assert.Equal( SpellRarity.Common, spell.Rarity ) );
        Assert.All( result, spell => Assert.Equal( kind, spell.Kind ) );
    }
}
