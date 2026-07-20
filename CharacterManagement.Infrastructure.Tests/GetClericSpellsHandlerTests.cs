using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.ClericSpells;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetClericSpellsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOrderedCatalogWithSelectionMetadata()
    {
        GetClericSpellsHandler handler = new GetClericSpellsHandler( new SpellRepository() );

        IReadOnlyCollection<SpellDefinitionDto> result = await handler.Handle(
            new GetClericSpellsCommand(),
            CancellationToken.None );

        Assert.Equal( 128, result.Count );
        SpellDefinitionDto sureStrike = Assert.Single(
            result,
            spell => spell.Id == "spell.sure_strike" );
        Assert.Equal( SpellKind.Spell, sureStrike.Kind );
        Assert.DoesNotContain( SpellTradition.Divine, sureStrike.Traditions );
        Assert.Equal( SpellRarity.Common, sureStrike.Rarity );
        Assert.Equal( "Player Core", sureStrike.Source.Book );
    }
}
